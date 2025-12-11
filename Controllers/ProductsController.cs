using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellinBE.Models;
using SellinBE.Models.Data;
using SellinBE.Models.Dtos;

namespace SellinBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly SellinProductionContext _context;

        public ProductsController(SellinProductionContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        private async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        private async Task<ActionResult<Product>> GetProducts(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        private async Task<IActionResult> PutProducts(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        private async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        private async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Products/5
        [HttpGet("Suggest/{customerId}")]
        public async Task<ActionResult<List<Product>>> SuggestProducts(int customerId)
        {
            var client = new HttpClient();

            var recommendedIds = await client.GetFromJsonAsync<List<int>>(
                   $"http://localhost:8000/recommend/{customerId}");

            if (recommendedIds == null || recommendedIds.Count == 0)
                return NotFound();

            List<Product> recommendedProducts = [];

            foreach (int id in recommendedIds)
            {
                var product = await _context.Products.FindAsync(id);

                if (product != null)
                {
                    recommendedProducts.Add(product);
                }
            }

            return recommendedProducts;
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

    }
}
