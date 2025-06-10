using OrderSystem.Domain.Models;

namespace OrderSystem.Infrastructure.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        
        Task<Client?> GetByIdAsync(int id);

        Task<IEnumerable<Client>> SearchByNameOrEmailAsync(string? search);

        Task<int> InsertAsync(Client client);

        Task<bool> UpdateAsync(Client client);

        Task<bool> DeleteAsync(int id);
    }
}
