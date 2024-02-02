using Backend.Dto;
using Backend.Entity;

namespace Backend.Service;

public interface ProductService
{   List<Product> GetAllProducts();
    Product GetProduct(int productId);
    Product AddProduct(Product product);
    void DeleteProduct(int productId);
    Product UpdateProduct(int productId, Product updatedProduct);
    
}