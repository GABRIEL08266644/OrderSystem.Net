using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.Services.Interfaces;
using OrderSystem.Domain.Models;
using OrderSystem.Infrastructure.Repositories.Interfaces;
using System.Data.Common;

namespace OrderSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IActionResult> Index(string? searchString, string? statusFilter, string? clientNameFilter)
        {
            var clients = await _clientService.GetAllAsync();

            ViewBag.StatusList = new List<string> { "Active", "Inactive", "Pending" };

            ViewBag.ClientNames = clients.Select(c => c.Name).Distinct().OrderBy(n => n).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }


            if (!string.IsNullOrEmpty(clientNameFilter))
            {
                clients = clients.Where(c => c.Name.Equals(clientNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            return View(clients);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                await _clientService!.InsertAsync(client);

                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Client? client = await _clientService.GetByIdAsync(id);

            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var updated = await _clientService!.UpdateAsync(client);
                if (!updated) return NotFound();

                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientService!.GetByIdAsync(id);

            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientService!.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            Client? client = await _clientService!.GetByIdAsync(id);

            if (client == null) return NotFound();
            return View(client);
        }
    }
}
