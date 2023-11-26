
using Backend.Dto;
using Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase 
{   
    private readonly ProductService _productService;
    
    public ProductController(ProductService productService) {
        _productService = productService;
    }

    [HttpGet]
    [Route("{productId}")]
    public IActionResult GetProduct(int productId) {
        ProductDto productDto = _productService.GetProduct(productId);
        return Ok(productDto);
    }
}