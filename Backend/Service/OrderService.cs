using Backend.Dao;
using Backend.Dto;
using Backend.Entity;
using Backend.ExceptionHandling;
using Microsoft.EntityFrameworkCore;
namespace Backend.Service;
public class OrderService(ApplicationDao db) {
    public List<OrderDto> GetUserOrders(string userId) => db.Orders.Include(o => o.Products)
        .Where(o => o.UserId == userId).OrderByDescending(o => o.OrderId).Select(o => Map(o)).ToList();
    private List<Product> ResolveProducts(List<int> ids) {
        if (ids.Count == 0 || ids.Count > 100 || ids.Distinct().Count() != ids.Count)
            throw new BadRequestException("Choose between 1 and 100 distinct products.");
        var products = db.Products.Where(p => ids.Contains(p.ProductId)).ToList();
        if (products.Count != ids.Count) throw new BadRequestException("One or more products do not exist.");
        return products;
    }
    public OrderDto CreateOrder(OrderDto dto, string userId) {
        var products = ResolveProducts(dto.ProductsIds);
        var order = new Order {UserId = userId, Products = products, TotalAmount = products.Sum(p => p.Price)};
        db.Orders.Add(order); db.SaveChanges(); return Map(order);
    }
    public OrderDto GetOrder(int id, string userId, bool admin = false) => Map(Find(id, userId, admin));
    private Order Find(int id, string userId, bool admin = false) => db.Orders.Include(o => o.Products)
        .FirstOrDefault(o => o.OrderId == id && (admin || o.UserId == userId)) ?? throw new KeyNotFoundException("Order not found.");
    public void DeleteOrder(int id, string userId) { var order = Find(id, userId); db.Orders.Remove(order); db.SaveChanges(); }
    public OrderDto UpdateOrder(int id, OrderDto dto, string userId) {
        var order = Find(id, userId); var products = ResolveProducts(dto.ProductsIds);
        order.Products.Clear(); order.Products.AddRange(products); order.TotalAmount = products.Sum(p => p.Price);
        db.SaveChanges(); return Map(order);
    }
    private static OrderDto Map(Order o) => new() {OrderId = o.OrderId, TotalAmount = o.TotalAmount, ProductsIds = o.Products.Select(p => p.ProductId).ToList()};
}
