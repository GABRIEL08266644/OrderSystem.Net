using System.Collections.Generic;
using System.Threading.Tasks;
using OrderSystem.Domain.Models;
using OrderSystem.Infra.Data.Repositories;

namespace OrderSystem.Services
{
    public class OrderListService
    {
        private readonly OrderListRepository _repository;

        public OrderListService(OrderListRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OrderList>> GetAllOrdersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<OrderList?> GetOrderByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddOrderAsync(OrderList order)
        {
            await _repository.CreateAsync(order);
        }

        public async Task UpdateOrderAsync(OrderList order)
        {
            await _repository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
