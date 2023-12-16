using Backend.Dto;

namespace Backend.Service;

public interface ProductService
{
    ProductDto AddProduct(ProductDto productDto);
    ProductDto GetProduct(int productId);
    void DeleteProduct(int productId);
    ProductDto UpdateProduct(int productId, ProductDto updatedProductDto);
    
}