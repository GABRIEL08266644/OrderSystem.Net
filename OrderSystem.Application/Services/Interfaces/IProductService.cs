using OrderSystem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSystem.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync(string? clientNameFilter = null);
        Task<IEnumerable<string>> GetAllClientNamesAsync();
        Task<Product?> GetByIdAsync(int id);
        Task InsertAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
