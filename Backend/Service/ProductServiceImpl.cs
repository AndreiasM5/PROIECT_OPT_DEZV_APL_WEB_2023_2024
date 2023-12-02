using Backend.Dao;
using Backend.Dto;
using Backend.Entity;

namespace Backend.Service;

public class ProductServiceImpl : ProductService
{
    private readonly ApplicationDao _applicationDao;

    public ProductServiceImpl(ApplicationDao applicationDao)
    {
        _applicationDao = applicationDao;
    }

    public ProductDto GetProduct(int productId)
    {   
        //saveProducts();
        Product product = _applicationDao.Products
        .FirstOrDefault(p => p.ProductId == productId);

        if (product == null) {
            throw new Exception();
        }
        
        ProductDto productDto = new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price
        };

        return productDto;
    }

    private void saveProducts() 
    {
        Product product1 = new Product
        {
            Name = "Water",
            Price = 2
        };
        Product product2 = new Product
        {
            Name = "Pepsi",
            Price = 4
        };
        Product product3 = new Product 
        {
            Name = "Jack Daniels",
            Price = 10
        };

        _applicationDao.Add(product1);
        _applicationDao.Add(product2);
        _applicationDao.Add(product3);
        _applicationDao.SaveChanges();
    }
}