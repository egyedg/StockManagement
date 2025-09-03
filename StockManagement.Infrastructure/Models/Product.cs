namespace StockManagement.Infrastructure.Models
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public Category Category { get; set; } = null!;
        public List<StockMovement> StockMovements { get; set; } = new();
    }
}