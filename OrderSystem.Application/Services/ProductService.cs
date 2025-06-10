using OrderSystem.Application.Services.Interfaces;
using OrderSystem.Domain.Models;
using OrderSystem.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSystem.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? clientNameFilter = null)
        {
            return await _productRepository.GetAllAsync(clientNameFilter);
        }

        public async Task<IEnumerable<string>> GetAllClientNamesAsync()
        {
            return await _productRepository.GetAllClientNamesAsync();
        }
        public async Task<IEnumerable<Product>> GetAllAsync(string? search = null, string? clientNameFilter = null)
        {
            var products = await _productRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Product product)
        {
            await _productRepository.InsertAsync(product);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            return await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
