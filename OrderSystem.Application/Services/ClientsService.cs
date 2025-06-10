using OrderSystem.Application.Services.Interfaces;
using OrderSystem.Domain.Models;
using OrderSystem.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;

namespace OrderSystem.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> GetAllAsync(string? searchString = null)
        {
            var clients = await _clientRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                clients = clients.Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            return (IEnumerable<Client>)clients;
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            return client == null ? null : new Client
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                RegistrationDate = client.RegistrationDate
            };
        }

        public async Task<int> InsertAsync(Client client)
        {
            return await _clientRepository.InsertAsync(client);
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            return await _clientRepository.UpdateAsync(client);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _clientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Client>> SearchAsync(string search)
        {
            return (IEnumerable<Client>)await _clientRepository.SearchByNameOrEmailAsync(search);
        }
    }
}
