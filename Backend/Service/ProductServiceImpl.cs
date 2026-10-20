using Backend.Dao;
using Backend.Entity;
using Backend.ExceptionHandling;
namespace Backend.Service;
public class ProductServiceImpl(ApplicationDao db) : ProductService {
    public List<Product> GetAllProducts() => db.Products.OrderBy(p => p.ProductId).ToList();
    public Product GetProduct(int id) => db.Products.Find(id) ?? throw new KeyNotFoundException("Product not found.");
    public Product AddProduct(Product product) {
        product.ProductId = 0; product.Name = product.Name.Trim(); db.Products.Add(product); db.SaveChanges(); return product;
    }
    public void DeleteProduct(int id) {
        var product = GetProduct(id);
        if (db.Orders.Any(o => o.Products.Any(p => p.ProductId == id))) throw new BadRequestException("This product is used in an order and cannot be deleted.");
        db.Products.Remove(product); db.SaveChanges();
    }
    public Product UpdateProduct(int id, Product updated) {
        var product = GetProduct(id); product.Name = updated.Name.Trim(); product.Price = updated.Price; db.SaveChanges(); return product;
    }
}
