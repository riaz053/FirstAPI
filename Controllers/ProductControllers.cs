using Microsoft.AspNetCore.Mvc;
using FirstApi.Data;
using FirstApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }

    // UPDATE


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        var existing = await _context.Products.FindAsync(id);

        if (existing == null)
            return NotFound();

        // ❌ DO NOT update Name or Id
        // existing.Name = product.Name;  ❌ REMOVE THIS

        // ✅ Only update allowed fields
        existing.Price = product.Price;
        existing.Quantity = product.Quantity;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok();
    }
}