namespace StockManagement.Infrastructure.Models
{
    public class Order
    {
        public string Id { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<StockMovement> StockMovements { get; set; } = new();
    }
}