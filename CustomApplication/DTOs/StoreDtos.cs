namespace CustomApplication.DTOs;

public record StoreProductDto(Guid Id, string StoreName, string Name, decimal PriceUsd, decimal WeightKg, string HsCode);

public record CargoCompanyDto(Guid Id, string Name, Guid WarehouseId, string WarehouseName);

public record StoreOrderItemRequest(Guid ProductId, int Quantity);

public record CreateStoreOrderRequest(List<StoreOrderItemRequest> Items, Guid? CargoCompanyId = null);

public record StoreOrderItemDto(
    Guid ProductId, string ProductName, string StoreName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record StoreOrderDto(
    Guid Id, string OrderNumber, DateTimeOffset CreatedAt, decimal TotalAmount,
    IReadOnlyList<StoreOrderItemDto> Items, Guid? ParcelId, string? TrackingNumber, string? CargoCompanyName);
