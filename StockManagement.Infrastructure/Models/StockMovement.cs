namespace StockManagement.Infrastructure.Models
{
    public class StockMovement
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public Product Product { get; set; } = null!;
        public string MovementType { get; set; } = string.Empty; // "IN" or "OUT"
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? OrderId { get; set; }
        public Order? Order { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}