using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        try
        {
            var product = new { id, name = "Sample Product", price = 99.99 };
            _logger.LogInformation("Retrieved product {ProductId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(product, "Product retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving product"));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = new[]
            {
                new { id = Guid.NewGuid(), name = "Product 1", price = 99.99 },
                new { id = Guid.NewGuid(), name = "Product 2", price = 149.99 }
            };

            return Ok(ApiResponse<object>.SuccessResponse(products, "Products retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving products"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var newProduct = new { id = Guid.NewGuid(), name = request.Name, price = request.Price };
            _logger.LogInformation("Product created: {ProductName}", request.Name);

            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.id },
                ApiResponse<object>.SuccessResponse(newProduct, "Product created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating product"));
        }
    }
}

public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
