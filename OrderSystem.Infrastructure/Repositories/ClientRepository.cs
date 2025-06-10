using Dapper;
using System.Data;
using OrderSystem.Domain.Models;
using OrderSystem.Infrastructure.Repositories.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnection _connection;

        public ClientRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            var sql = "SELECT * FROM Clients";
            return await _connection.QueryAsync<Client>(sql);
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Clients WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Client>(sql, new { Id = id });
        }

        public async Task<int> InsertAsync(Client client)
        {
            var sql = @"INSERT INTO Clients (Name, Email, Phone, RegistrationDate)
                        VALUES (@Name, @Email, @Phone, GETDATE());
                        SELECT CAST(SCOPE_IDENTITY() as int)";
            
            return await _connection.ExecuteScalarAsync<int>(sql, client);
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            var sql = @"UPDATE Clients SET Name = @Name, Email = @Email, Phone = @Phone
                        WHERE Id = @Id";
            var rows = await _connection.ExecuteAsync(sql, client);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Clients WHERE Id = @Id";
            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<IEnumerable<Client>> SearchByNameOrEmailAsync(string? search)
        {
            var sql = "SELECT * FROM Clients";

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " WHERE Name LIKE @search OR Name LIKE @search";
            }

            return await _connection.QueryAsync<Client>(sql, new { search = $"%{search}%" });
        }
    }
}
