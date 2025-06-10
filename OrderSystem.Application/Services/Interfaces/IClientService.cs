using OrderSystem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSystem.Application.Services.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllAsync(string? searchString = null);
        Task<Client?> GetByIdAsync(int id);
        Task<int> InsertAsync(Client client);
        Task<bool> UpdateAsync(Client client);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Client>> SearchAsync(string search);
    }
}
