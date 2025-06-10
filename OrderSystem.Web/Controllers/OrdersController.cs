using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderSystem.Application.Services;
using OrderSystem.Domain.Models;
using OrderSystem.Infra.Data.Repositories;
using OrderSystem.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSystem.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderListService _orderListService;
        private readonly ClientService _clientService;
        private readonly ProductService _productService;
        private readonly OrderListRepository _orderRepository;

        public OrdersController(
            OrderListService orderListService, 
            ClientService clientService, 
            ProductService productService, 
            OrderListRepository orderRepository
        )
        {
            _orderListService = orderListService;
            _clientService = clientService;
            _productService = productService;
            _orderRepository = orderRepository;
        }
        
        public async Task<IActionResult> Index(string clientNameFilter, string statusFilter)
        {
            var orders = await _orderRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(clientNameFilter))
                orders = orders.Where(o => o.Client.Name == clientNameFilter).ToList();

            if (!string.IsNullOrEmpty(statusFilter))
                orders = orders.Where(o => o.Status == statusFilter).ToList();

            ViewBag.ClientNames = orders.Select(o => o.Client.Name).Distinct().ToList();
            ViewBag.StatusList = orders.Select(o => o.Status).Distinct().ToList();

            return View(orders);
        }
        
        public async Task<IActionResult> Create()
        {
            await LoadClients();
            await LoadProducts();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderList order)
        {
            if (order.ProductIds == null || !order.ProductIds.Any())
            {
                ModelState.AddModelError("ProductIds", "Selecione pelo menos um produto.");
            }

            if (!ModelState.IsValid)
            {
                await LoadClients();
                await LoadProducts();
                return View(order);
            }
            
            order.Client = await _clientService.GetByIdAsync(order.ClientId);
            if (order.Client == null)
            {
                ModelState.AddModelError("ClientId", "Cliente inválido.");
                await LoadClients();
                await LoadProducts();
                return View(order);
            }
            
            var items = new List<OrderItem>();
            foreach (var productId in order.ProductIds)
            {
                
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                {
                    ModelState.AddModelError("ProductIds", $"Produto com ID {productId} não encontrado.");
                    await LoadClients();
                    await LoadProducts();
                    return View(order);
                }

                items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = order.Quantity, 
                    UnitPrice = product.Price
                });
            }

            order.Items = items;
            order.OrderDate = DateTime.Now; 

            
            await _orderListService.AddOrderAsync(order);

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderListService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await LoadProducts();

            var clients = await _clientService.GetAllAsync();
            ViewBag.Clients = clients.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            return View(order);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderList order)
        {
            if (order.ProductIds == null || !order.ProductIds.Any())
            {
                ModelState.AddModelError("ProductIds", "Selecione pelo menos um produto.");
            }

            if (!ModelState.IsValid)
            {
                await LoadClients();
                await LoadProducts();
                return View(order);
            }
            
            order.Client = await _clientService.GetByIdAsync(order.ClientId);
            if (order.Client == null)
            {
                ModelState.AddModelError("ClientId", "Cliente inválido.");
                await LoadClients();
                await LoadProducts();
                return View(order);
            }
            
            var items = new List<OrderItem>();
            foreach (var productId in order.ProductIds)
            {
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                {
                    ModelState.AddModelError("ProductIds", $"Produto com ID {productId} não encontrado.");
                    await LoadClients();
                    await LoadProducts();
                    return View(order);
                }

                items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = order.Quantity, 
                    UnitPrice = product.Price
                });
            }
            order.Items = items;

            
            await _orderListService.UpdateOrderAsync(order);

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderListService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderListService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderListService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        private async Task LoadClients()
        {
            var clients = await _clientService.GetAllAsync();
            ViewBag.Clients = clients
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
        }

        private async Task LoadProducts()
        {
            var products = await _productService.GetAllAsync(search: null) ?? new List<Product>();
            ViewBag.Products = new SelectList(products, "Id", "Name");
        }
    }
}
