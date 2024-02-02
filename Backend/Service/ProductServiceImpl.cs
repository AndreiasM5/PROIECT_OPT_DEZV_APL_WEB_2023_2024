using Backend.Dao;
using Backend.Dto;
using Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

public class ProductServiceImpl : ProductService
{
    private readonly ApplicationDao _applicationDao;

    public ProductServiceImpl(ApplicationDao applicationDao)
    {
        _applicationDao = applicationDao;
    }

    public List<Product> GetAllProducts()
    {
        List<Product> products = _applicationDao.Products.ToList();

        return products;
    }

    public Product GetProduct(int productId)
    {   
        Product product = _applicationDao.Products
        .FirstOrDefault(p => p.ProductId == productId);

        if (product == null) {
            throw new KeyNotFoundException("product with specified id doesn't exist");
        }

        return product;
    }
    
    public Product AddProduct(Product product)
    {   
        _applicationDao.Products.Add(product);
        _applicationDao.SaveChanges();

        product.ProductId = product.ProductId;
        return product;
    }

    public void DeleteProduct(int productId) 
    {

        Product product = _applicationDao.Products
        .FirstOrDefault(p => p.ProductId == productId);

        if (product == null) {
            throw new Exception();
        }
        
        _applicationDao.Products.Remove(product);
        _applicationDao.SaveChanges();
        
    }

    public Product UpdateProduct(int productId, Product updatedProduct)
    {
    // Retrieve the existing product
    Product existingProduct = _applicationDao.Products
        .FirstOrDefault(p => p.ProductId == productId);

    // Check if the product exists
    if (existingProduct == null)
    {
        throw new Exception("Product not found");
    }

    // Update the existing product with the new data
    existingProduct.Name = updatedProduct.Name;
    existingProduct.Price = updatedProduct.Price;

    // Save changes to the database
    _applicationDao.SaveChanges();

    return existingProduct;
    }

}