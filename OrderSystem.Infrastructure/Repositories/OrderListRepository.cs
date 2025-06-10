using Dapper;
using OrderSystem.Domain.Models;
using System.Data;

namespace OrderSystem.Infra.Data.Repositories
{
    public class OrderListRepository
    {
        private readonly IDbConnection _connection;

        public OrderListRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        private async Task<bool> ClientExistsAsync(int clientId)
        {
            var sql = "SELECT COUNT(1) FROM Clients WHERE Id = @Id";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Id = clientId });
            return count > 0;
        }

        private async Task<bool> ValidateStockAsync(IEnumerable<OrderItem> items)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();

            var sql = @"SELECT Id, StockQuantity FROM Products WHERE Id IN @Ids";

            var stocks = (await _connection.QueryAsync<Product>(sql, new { Ids = productIds }))
                         .ToDictionary(p => p.Id, p => p.StockQuantity);

            foreach (var item in items)
            {
                if (!stocks.TryGetValue(item.ProductId, out var stock) || stock < item.Quantity)
                {
                }
            }

            return true;
        }

        private decimal CalculateTotal(IEnumerable<OrderItem> items)
        {
            return items.Sum(i => i.Quantity * i.UnitPrice);
        }

        private async Task UpdateStockAsync(IEnumerable<OrderItem> items, IDbTransaction transaction)
        {
            var sqlUpdateStock = @"
                UPDATE Products
                SET StockQuantity = StockQuantity - @Quantity
                WHERE Id = @ProductId";

            foreach (var item in items)
            {
                await _connection.ExecuteAsync(sqlUpdateStock, new
                {
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                }, transaction);
            }
        }

        public async Task<int> CreateAsync(OrderList order)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            if (order.Items == null || !order.Items.Any())
                throw new ArgumentException("O pedido deve conter pelo menos um item.");

            if (!await ClientExistsAsync(order.ClientId))
                throw new InvalidOperationException("Cliente não encontrado.");

            if (!await ValidateStockAsync(order.Items))
                throw new InvalidOperationException("Estoque insuficiente para um ou mais produtos.");

            order.TotalAmount = CalculateTotal(order.Items);

            if (string.IsNullOrEmpty(order.Status))
                order.Status = "Novo";

            using var transaction = _connection.BeginTransaction();

            try
            {
                var sqlInsertOrder = @"
                    INSERT INTO Orders (ClientId, OrderDate, TotalAmount, Status)
                    VALUES (@ClientId, @OrderDate, @TotalAmount, @Status);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var orderId = await _connection.ExecuteScalarAsync<int>(sqlInsertOrder, new
                {
                    order.ClientId,
                    order.OrderDate,
                    order.TotalAmount,
                    order.Status
                }, transaction);

                var sqlInsertItem = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

                foreach (var item in order.Items)
                {

                    await _connection.ExecuteAsync(sqlInsertItem, new
                    {
                        OrderId = orderId,
                        item.ProductId,
                        item.Quantity,
                        item.UnitPrice
                    }, transaction);
                }

                await UpdateStockAsync(order.Items, transaction);

                transaction.Commit();

                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<IEnumerable<OrderList>> GetAllAsync()
        {
            var sql = @"
                SELECT 
                    o.Id, o.ClientId, o.OrderDate, o.TotalAmount, o.Status,
                    c.Id as ClientId, c.Name,
                    oi.Id as OrderItemId, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                    p.Id as ProductId, p.Name, p.Price, p.StockQuantity
                FROM Orders o
                INNER JOIN Clients c ON o.ClientId = c.Id
                LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
                LEFT JOIN Products p ON p.Id = oi.ProductId
                ORDER BY o.OrderDate DESC";

            var orderDict = new Dictionary<int, OrderList>();

            var list = await _connection.QueryAsync<OrderList, Client, OrderItem, Product, OrderList>(
                sql,
                (order, client, orderItem, product) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Client = client;
                        currentOrder.Quantity = 0;
                        currentOrder.Items = new List<OrderItem>();
                        orderDict.Add(currentOrder.Id, currentOrder);
                    }

                    if (orderItem != null)
                    {
                        orderItem.Product = product;
                        currentOrder.Items.Add(orderItem);
                        currentOrder.Quantity += orderItem.Quantity;
                    }

                    return currentOrder;
                },
                splitOn: "ClientId,OrderItemId,ProductId"
            );

            return orderDict.Values;
        }

        public async Task<OrderList?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT 
                    o.Id, o.ClientId, o.OrderDate, o.TotalAmount, o.Status,
                    c.Id as ClientId, c.Name,
                    oi.Id as OrderItemId, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                    p.Id as ProductId, p.Name, p.Price, p.StockQuantity
                FROM Orders o
                INNER JOIN Clients c ON o.ClientId = c.Id
                LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
                LEFT JOIN Products p ON p.Id = oi.ProductId
                WHERE o.Id = @Id";

            var orderDict = new Dictionary<int, OrderList>();

            var list = await _connection.QueryAsync<OrderList, Client, OrderItem, Product, OrderList>(
                sql,
                (order, client, orderItem, product) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Client = client;
                        currentOrder.Items = new List<OrderItem>();
                        orderDict.Add(currentOrder.Id, currentOrder);
                    }

                    if (orderItem != null)
                    {
                        orderItem.Product = product;
                        currentOrder.Items.Add(orderItem);
                    }

                    return currentOrder;
                },
                new { Id = id },
                splitOn: "ClientId,OrderItemId,ProductId"
            );

            return orderDict.Values.FirstOrDefault();
        }

        public async Task UpdateAsync(OrderList order)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            if (order.Items == null || !order.Items.Any())
                throw new ArgumentException("O pedido deve conter pelo menos um item.");

            if (!await ClientExistsAsync(order.ClientId))
                throw new InvalidOperationException("Cliente não encontrado.");

            if (!await ValidateStockAsync(order.Items))
                throw new InvalidOperationException("Estoque insuficiente para um ou mais produtos.");

            order.TotalAmount = CalculateTotal(order.Items);

            using var transaction = _connection.BeginTransaction();

            try
            {
                var sqlUpdateOrder = @"
                    UPDATE Orders
                    SET 
                        ClientId = @ClientId,
                        OrderDate = @OrderDate,
                        TotalAmount = @TotalAmount,
                        Quantity = @Quantity,
                        Status = @Status
                    WHERE Id = @Id";

                await _connection.ExecuteAsync(sqlUpdateOrder, new
                {
                    order.ClientId,
                    order.OrderDate,
                    order.TotalAmount,
                    order.Quantity,
                    order.Status,
                    order.Id
                }, transaction);

                var sqlDeleteItems = @"DELETE FROM OrderItems WHERE OrderId = @OrderId";
                await _connection.ExecuteAsync(sqlDeleteItems, new { OrderId = order.Id }, transaction);

                if (order.Items != null && order.Items.Any())
                {
                    var sqlInsertItem = @"
                        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                        VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

                    foreach (var item in order.Items)
                    {
                        await _connection.ExecuteAsync(sqlInsertItem, new
                        {
                            OrderId = order.Id,
                            item.ProductId,
                            item.Quantity,
                            item.UnitPrice
                        }, transaction);
                    }
                }

                await UpdateStockAsync(order.Items, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var sqlDeleteItems = @"DELETE FROM OrderItems WHERE OrderId = @OrderId";
            await _connection.ExecuteAsync(sqlDeleteItems, new { OrderId = id });

            var sqlDeleteOrder = @"DELETE FROM Orders WHERE Id = @Id";
            await _connection.ExecuteAsync(sqlDeleteOrder, new { Id = id });
        }
    }
}
