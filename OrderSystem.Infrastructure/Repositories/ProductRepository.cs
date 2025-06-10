using OrderSystem.Domain.Models;
using OrderSystem.Infrastructure.Repositories.Interfaces;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrderSystem.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? search = null, string? clientNameFilter = null)
        {
            var sql = "SELECT * FROM Products WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND (Name LIKE @Search OR Description LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrEmpty(clientNameFilter))
            {
                sql += " AND ClientName = @ClientName";
                parameters.Add("ClientName", clientNameFilter);
            }

            return await _connection.QueryAsync<Product>(sql, parameters);
        }

        public async Task<IEnumerable<string>> GetAllClientNamesAsync()
        {
            var sql = "SELECT DISTINCT Name FROM Products ORDER BY Name";
            return await _connection.QueryAsync<string>(sql);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Products WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<int> InsertAsync(Product product)
        {
            var sql = @"
                INSERT INTO Products (Name, Description, Price, StockQuantity) 
                VALUES (@Name, @Description, @Price, @StockQuantity);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = await _connection.ExecuteScalarAsync<int>(sql, product);
            return id;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var sql = @"
                UPDATE Products 
                SET Name = @Name, Description = @Description, Price = @Price, StockQuantity = @StockQuantity 
                WHERE Id = @Id";
            var rows = await _connection.ExecuteAsync(sql, product);
            return rows > 0;
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
