using Microsoft.AspNetCore.Mvc;
using StockManagement.API.Services;

namespace StockManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        
        public StockController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductStock(string productId)
        {
            var stock = await _mongoDBService.GetProductStockAsync(productId);
            return Ok(stock);
        }

        [HttpPost("movement")]
        public async Task<IActionResult> AddStockMovement([FromBody] StockMovement movement)
        {
            await _mongoDBService.AddStockMovementAsync(movement);
            return Ok();
        }

        [HttpGet("product/{productId}/movements")]
        public async Task<IActionResult> GetProductMovements(string productId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var movements = await _mongoDBService.GetProductMovementsAsync(productId, startDate, endDate);
            return Ok(movements);
        }
    }
}