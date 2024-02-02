using Backend.Entity;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase 
{   
    private readonly ProductService _productService;
    
    public ProductController(ProductService productService) {
        _productService = productService;
    }

    [HttpGet]
    [Route("all")]
    public IActionResult GetAllProducts() {
        List<Product> products = _productService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet]
    [Route("{productId}")]
    public IActionResult GetProduct(int productId) {
        Product product = _productService.GetProduct(productId);
        return Ok(product);
    }

    [HttpPost]  
    public IActionResult AddCustomer([FromBody] Product Product)
    {
        Product = _productService.AddProduct(Product);
        
        return CreatedAtAction(nameof(GetProduct), new { Product.ProductId }, Product );
    }

    [HttpDelete]
    [Route("{productId}")]
    public IActionResult DeleteProduct(int productId) {
        _productService.DeleteProduct(productId);
        return NoContent();
    }

    [HttpPut]
    [Route("{productId}")]
    public IActionResult UpdateProduct(int productId, [FromBody] Product updatedProduct)
    {
        Product updatedDto = _productService.UpdateProduct(productId, updatedProduct);
        return Ok(updatedDto);
    }
    
}