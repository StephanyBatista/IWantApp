namespace IWantApp.Endpoints.Products;

public record ProductResponse(Guid Id, string Name, string CategoryName, string Description, bool HasStock, decimal Price, bool Active);
