using OrderSystem.Domain.Models;

namespace OrderSystem.Infrastructure.Repositories.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> GetAllAsync();
        
        Task<OrderItem?> GetByIdAsync(int id);

        Task<int> CreateAsync(OrderItem orderItem);

        Task<bool> UpdateAsync(OrderItem orderItem);

        Task<bool> DeleteAsync(int id);
    }
}
