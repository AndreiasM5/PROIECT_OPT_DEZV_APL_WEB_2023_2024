using Backend.Dto;

namespace Backend.Service;

public interface ProductService
{
    ProductDto GetProduct(int productId);
}