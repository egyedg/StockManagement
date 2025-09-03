using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace StockManagement.API.Services
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }

    public class MongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        private IMongoCollection<ProductStock> ProductStocksCollection => 
            _database.GetCollection<ProductStock>("ProductStocks");
            
        private IMongoCollection<StockMovement> StockMovementsCollection => 
            _database.GetCollection<StockMovement>("StockMovements");

        public async Task<ProductStock?> GetProductStockAsync(string productId)
        {
            return await ProductStocksCollection
                .Find(p => p.ProductId == productId)
                .FirstOrDefaultAsync();
        }

        public async Task AddStockMovementAsync(StockMovement movement)
        {
            // Start a transaction
            using var session = await _database.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                // Add the movement
                await StockMovementsCollection.InsertOneAsync(session, movement);

                // Update the product stock
                var filter = Builders<ProductStock>.Filter.Eq(p => p.ProductId, movement.ProductId);
                var update = Builders<ProductStock>.Update
                    .Inc(p => p.CurrentStock, movement.MovementType == "IN" ? movement.Quantity : -movement.Quantity)
                    .Set(p => p.LastUpdated, DateTime.UtcNow);

                var options = new FindOneAndUpdateOptions<ProductStock>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

                await ProductStocksCollection.FindOneAndUpdateAsync(
                    session, filter, update, options);

                await session.CommitTransactionAsync();
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task<List<StockMovement>> GetProductMovementsAsync(string productId, DateTime? startDate, DateTime? endDate)
        {
            var filter = Builders<StockMovement>.Filter.Eq(m => m.ProductId, productId);
            
            if (startDate.HasValue)
                filter &= Builders<StockMovement>.Filter.Gte(m => m.Timestamp, startDate.Value);
            
            if (endDate.HasValue)
                filter &= Builders<StockMovement>.Filter.Lte(m => m.Timestamp, endDate.Value);

            return await StockMovementsCollection
                .Find(filter)
                .SortByDescending(m => m.Timestamp)
                .ToListAsync();
        }
    }

    public class ProductStock
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class StockMovement
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? ReferenceOrderId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}