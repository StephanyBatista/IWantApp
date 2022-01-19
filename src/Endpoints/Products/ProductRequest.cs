namespace IWantApp.Endpoints.Products;

public record ProductRequest(string Name, Guid CategoryId, string Description, bool HasStock, decimal Price, bool Active);
