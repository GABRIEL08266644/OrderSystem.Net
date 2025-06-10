using OrderSystem.Domain.Models;

namespace OrderSystem.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();

        Task<Order?> GetByIdAsync(int id);

        Task<int> CreateAsync(Order order);

        Task<bool> UpdateAsync(Order order);
        
        Task<bool> DeleteAsync(int id);
    }
}
