namespace IWantApp.Endpoints.Clients;

public record OrderRequest(List<Guid> ProductIds, string DeliveryAddress);
