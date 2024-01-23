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

    public ProductDto GetProduct(int productId)
    {   
        Product product = _applicationDao.Products
        .FirstOrDefault(p => p.ProductId == productId);

        if (product == null) {
            throw new KeyNotFoundException("product with specified id doesn't exist");
        }
        
        ProductDto productDto = new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price
        };

        return productDto;
    }
    public ProductDto AddProduct(ProductDto productDto)
    {   
        Product product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price
        };

        _applicationDao.Products.Add(product);
        _applicationDao.SaveChanges();

        productDto.ProductId = product.ProductId;
        return productDto;
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

    public ProductDto UpdateProduct(int productId, ProductDto updatedProductDto)
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
    existingProduct.Name = updatedProductDto.Name;
    existingProduct.Price = updatedProductDto.Price;

    // Save changes to the database
    _applicationDao.SaveChanges();

    // Return the updated product data
    updatedProductDto = new ProductDto
    {
        ProductId = existingProduct.ProductId,
        Name = existingProduct.Name,
        Price = existingProduct.Price
    };

    return updatedProductDto;
    }


}