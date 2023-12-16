
using Backend.Dto;
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
    [Route("{productId}")]
    public IActionResult GetProduct(int productId) {
        ProductDto productDto = _productService.GetProduct(productId);
        return Ok(productDto);
    }

    [HttpPost]  
    public IActionResult AddCustomer([FromBody] ProductDto productDto)
    {
        productDto = _productService.AddProduct(productDto);
        
        return CreatedAtAction(nameof(GetProduct), new { productDto.ProductId }, productDto );
    }

    [HttpDelete]
    [Route("{productId}")]
    public IActionResult DeleteProduct(int productId) {
        _productService.DeleteProduct(productId);
        return NoContent();
    }

    [HttpPut]
    [Route("{productId}")]
    public IActionResult UpdateProduct(int productId, [FromBody] ProductDto updatedProductDto)
    {
        ProductDto updatedDto = _productService.UpdateProduct(productId, updatedProductDto);
        return Ok(updatedDto);
    }
    
}