using DailyGlowApi.Dtos;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyGlowApi.Controllers;

// Handles the /api/products endpoints.
// It only deals with HTTP. The logic is in the service.
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    // GET api/products?category=Serum&skinType=Oily&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll([FromQuery] ProductQueryParameters query)
    {
        var products = await _service.GetAllAsync(query);
        return Ok(products);
    }

    // GET api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    // POST api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var created = await _service.CreateAsync(dto);

        // 201 Created, with a Location header that points to the new product
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/products/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE api/products/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        return result switch
        {
            DeleteProductResult.Deleted => NoContent(),
            DeleteProductResult.InUse => Problem(
                title: "Product is in use",
                detail: "This product is used in a routine. Remove it from the routine first.",
                statusCode: StatusCodes.Status409Conflict),
            _ => NotFound()
        };
    }
}
