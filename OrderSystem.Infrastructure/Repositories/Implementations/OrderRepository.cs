using Dapper;
using OrderSystem.Domain.Models;
using OrderSystem.Infrastructure.Repositories.Interfaces;
using System.Data;

namespace OrderSystem.Infrastructure.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _connection;

        public OrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var sql = @"
                SELECT o.*, i.*
                FROM Orders o
                LEFT JOIN OrderItems i ON i.OrderId = o.Id";

            var orderDict = new Dictionary<int, Order>();

            var result = await _connection.QueryAsync<Order, OrderItem, Order>(
                sql,
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Items = new List<OrderItem>();
                        orderDict.Add(currentOrder.Id, currentOrder);
                    }

                    if (item != null)
                    {
                        currentOrder.Items.Add(item);
                    }

                    return currentOrder;
                },
                splitOn: "Id"
            );

            return orderDict.Values;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT o.*, i.*
                FROM Orders o
                LEFT JOIN OrderItems i ON i.OrderId = o.Id
                WHERE o.Id = @Id";

            var orderDict = new Dictionary<int, Order>();

            var result = await _connection.QueryAsync<Order, OrderItem, Order>(
                sql,
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Items = new List<OrderItem>();
                        orderDict.Add(currentOrder.Id, currentOrder);
                    }

                    if (item != null)
                    {
                        currentOrder.Items.Add(item);
                    }

                    return currentOrder;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return orderDict.Values.FirstOrDefault();
        }

        public async Task<int> CreateAsync(Order order)
        {
            var insertOrderSql = @"
                INSERT INTO Orders (CustomerName, OrderDate, ...)
                VALUES (@CustomerName, @OrderDate, ...);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var newId = await _connection.QuerySingleAsync<int>(insertOrderSql, order);
            order.Id = newId;

            foreach (var item in order.Items)
            {
                item.OrderId = newId;

                var insertItemSql = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, ...)
                    VALUES (@OrderId, @ProductId, @Quantity, ...);";

                await _connection.ExecuteAsync(insertItemSql, item);
            }

            return newId;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            var updateOrderSql = @"
                UPDATE Orders
                SET CustomerName = @CustomerName,
                    OrderDate = @OrderDate
                    -- outros campos
                WHERE Id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(updateOrderSql, order);

            var deleteItemsSql = "DELETE FROM OrderItems WHERE OrderId = @OrderId";
            await _connection.ExecuteAsync(deleteItemsSql, new { OrderId = order.Id });

            foreach (var item in order.Items)
            {
                item.OrderId = order.Id;

                var insertItemSql = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, ...)
                    VALUES (@OrderId, @ProductId, @Quantity, ...);";

                await _connection.ExecuteAsync(insertItemSql, item);
            }

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleteItemsSql = "DELETE FROM OrderItems WHERE OrderId = @OrderId";
            await _connection.ExecuteAsync(deleteItemsSql, new { OrderId = id });

            var deleteOrderSql = "DELETE FROM Orders WHERE Id = @Id";
            var rowsAffected = await _connection.ExecuteAsync(deleteOrderSql, new { Id = id });

            return rowsAffected > 0;
        }
    }
}
