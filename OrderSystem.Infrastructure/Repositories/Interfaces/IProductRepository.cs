using OrderSystem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSystem.Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(string? search = null, string? clientNameFilter = null);

        Task<IEnumerable<string>> GetAllClientNamesAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<int> InsertAsync(Product product);

        Task<bool> UpdateAsync(Product product);
        
        Task DeleteAsync(int id);
    }
}
