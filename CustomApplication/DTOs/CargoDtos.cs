using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.DTOs
{
    public record CargoParcelQuery(int Page = 1, int PageSize = 20, CargoParcelStatus? Status = null);

    public record CargoParcelListItemDto(
        Guid Id, string TrackingNumber, CargoParcelStatus Status, DateTimeOffset ArrivedAt,
        string WarehouseName, string OwnerName, decimal TotalValue, int ItemCount, Guid? DeclarationId, string? CargoCompanyName);

    public record CargoParcelItemDto(
        string ProductName, string StoreName, string HsCode, int Quantity, decimal UnitPrice, decimal WeightKg);

    public record CargoParcelDetailDto(
        Guid Id, string TrackingNumber, string OrderNumber, CargoParcelStatus Status, DateTimeOffset ArrivedAt,
        string WarehouseName, string OwnerName, decimal TotalValue,
        Guid? DeclarationId, DateTimeOffset? DeclaredAt, IReadOnlyList<CargoParcelItemDto> Items, string? CargoCompanyName);

}
