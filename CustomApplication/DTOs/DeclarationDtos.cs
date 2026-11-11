using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.DTOs
{

    public record DeclarationItemRequest(string HsCode, string Description, int Quantity, decimal UnitPrice, decimal WeightKg);
    public record SaveDeclarationRequest(Guid WarehouseId, List<DeclarationItemRequest> Items);
    public record ChangeStatusRequest(DeclarationStatus TargetStatus, string? Reason);
    public record DeclarationQuery(int Page = 1, int PageSize = 20, DeclarationStatus? Status = null, string? Search = null);

    public record DeclarationScope(Guid? OwnerId, bool ExcludeDrafts)
    {
        public static readonly DeclarationScope All = new(null, false);
    }

    public record DeclarationListItemDto(
        Guid Id, string Number, DeclarationStatus Status, DeclarantType DeclarantType,
        string DeclarantName, decimal TotalPayable, DateTimeOffset CreatedAt, int ItemCount);

    public record DeclarationItemDto(
        Guid Id, string HsCode, string Description, int Quantity, decimal UnitPrice,
        decimal CustomsValue, decimal WeightKg, decimal DutyAmount, decimal VatAmount);

    public record StatusHistoryDto(
        DeclarationStatus? From, DeclarationStatus To, string ChangedBy, DateTimeOffset ChangedAt, string? Reason);

    public record DeclarationDetailDto(
        Guid Id, string Number, DeclarationStatus Status, DeclarantType DeclarantType,
        string DeclarantName, Guid WarehouseId, string WarehouseName,
        decimal TotalCustomsValue, decimal ExemptAmount, decimal TotalDuty, decimal TotalVat, decimal TotalPayable,
        DateTimeOffset CreatedAt, DateTimeOffset? SubmittedAt,
        IReadOnlyList<DeclarationItemDto> Items, IReadOnlyList<StatusHistoryDto> Timeline);

}
