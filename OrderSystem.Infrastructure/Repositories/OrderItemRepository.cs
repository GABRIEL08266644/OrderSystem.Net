using Dapper;
using System.Data;
using OrderSystem.Domain.Models;

public class OrderItemRepository
{
    private readonly IDbConnection _connection;

    public OrderItemRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<OrderItem> GetByOrderId(int orderId)
    {
        string sql = @"SELECT * FROM OrderItems WHERE OrderId = @OrderId";
        return _connection.Query<OrderItem>(sql, new { OrderId = orderId });
    }

    public void Add(OrderItem item)
    {
        string sql = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) 
                       VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";
        _connection.Execute(sql, item);
    }
}
