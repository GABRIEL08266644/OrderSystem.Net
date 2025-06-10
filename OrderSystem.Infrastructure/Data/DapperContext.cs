

using Microsoft.Data.SqlClient;
using System.Data;

namespace OrderSystem.Infrastructure.Data
{
    public class DapperContext
    {
        private readonly string  _connectionString;
        public DapperContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
