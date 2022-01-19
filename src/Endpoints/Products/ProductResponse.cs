namespace IWantApp.Endpoints.Products;

public record ProductResponse(string Name, string CategoryName, string Description, bool HasStock, bool Active);
