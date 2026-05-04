using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FirstApi.Data;
using FirstApi.Models;

namespace FirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔐 All endpoints require login
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // 📄 GET ALL (Admin + User)
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    // 📄 GET BY ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // ➕ CREATE (Admin + User)
    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Create(Product product)
    {
        // 🔥 Optional: track creator
        var userId = User.FindFirst("UserId")?.Value;
        if (userId != null)
        {
            product.CreatedByUserId = int.Parse(userId);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(product);
    }

    // ✏️ UPDATE (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Product updatedProduct)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        // 🔒 Do NOT update Name & Id (your requirement)
        product.Price = updatedProduct.Price;
        product.Quantity = updatedProduct.Quantity;

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    // ❌ DELETE (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Deleted successfully" });
    }
}




// [Authorize]   // 🔐 protects all endpoints
// [ApiController]
// [Route("api/[controller]")]
// public class ProductsController : ControllerBase
// {
//     private readonly AppDbContext _context;

//     public ProductsController(AppDbContext context)
//     {
//         _context = context;
//     }

//     [HttpGet]
//     public async Task<IActionResult> GetAll()
//     {
//         return Ok(await _context.Products.ToListAsync());
//     }

//     [HttpPost]
//     public async Task<IActionResult> Create([FromBody] Product product)
//     {
//         _context.Products.Add(product);
//         await _context.SaveChangesAsync();
//         return Ok(product);
//     }

//     // UPDATE


//     [HttpPut("{id}")]
//     public async Task<IActionResult> Update(int id, [FromBody] Product product)
//     {
//         var existing = await _context.Products.FindAsync(id);

//         if (existing == null)
//             return NotFound();

//         // ❌ DO NOT update Name or Id
//         // existing.Name = product.Name;  ❌ REMOVE THIS

//         // ✅ Only update allowed fields
//         existing.Price = product.Price;
//         existing.Quantity = product.Quantity;

//         await _context.SaveChangesAsync();

//         return Ok(existing);
//     }

//     // DELETE
//     [HttpDelete("{id}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         var product = await _context.Products.FindAsync(id);

//         if (product == null)
//             return NotFound();

//         _context.Products.Remove(product);
//         await _context.SaveChangesAsync();

//         return Ok();
//     }
// }


