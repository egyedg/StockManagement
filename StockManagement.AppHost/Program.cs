var builder = DistributedApplication.CreateBuilder(args);

// Add MongoDB container
var mongo = builder.AddMongoDB("mongo");
var mongodb = mongo.AddDatabase("mongodb");

// Add PostgreSQL container
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume();

var db = postgres.AddDatabase("stockdb");

// Add the API service
var apiService = builder.AddProject<Projects.StockManagement_API>("api")
    .WithReference(mongodb)
    .WithReference(db);

builder.Build().Run();