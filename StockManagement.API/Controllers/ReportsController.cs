using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Infrastructure.Data;

namespace StockManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stock-movements")]
        public async Task<IActionResult> GetStockMovementsReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate,
            [FromQuery] string? productId = null,
            [FromQuery] string? movementType = null)
        {
            var query = _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.Order)
                .Where(sm => sm.Timestamp >= startDate && sm.Timestamp <= endDate);

            if (!string.IsNullOrEmpty(productId))
                query = query.Where(sm => sm.ProductId == productId);

            if (!string.IsNullOrEmpty(movementType))
                query = query.Where(sm => sm.MovementType == movementType);

            var movements = await query.ToListAsync();
            return Ok(movements);
        }

        [HttpGet("category/{categoryId}/products")]
        public async Task<IActionResult> GetProductsByCategory(string categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
            
            return Ok(products);
        }
    }
}