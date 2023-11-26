using Backend.Dto;

namespace Backend.Service;

public class ProductServiceImpl : ProductService
{
    public ProductDto GetProduct(int productId)
    {
        ProductDto productDto1 = new ProductDto 
        {
            ProductId = 1,
            Name = "Water",
            Price = 2
        };
        ProductDto productDto2 = new ProductDto 
        {
            ProductId = 2,
            Name = "Pepsi",
            Price = 4
        };
        ProductDto productDto3 = new ProductDto 
        {
            ProductId = 3,
            Name = "Jack Daniels",
            Price = 10
        };

        if (productId == 1) {
            return productDto1;
        }
        if (productId == 2) {
            return productDto2;
        }
        else {
            return productDto3;
        }
    }
}