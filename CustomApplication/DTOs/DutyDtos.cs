using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.DTOs
{
    public record DutyItemRequest(string HsCode, int Quantity, decimal UnitPrice);

    public record CalculateDutyRequest(DeclarantType DeclarantType, List<DutyItemRequest> Items);

    public record DutyLineDto(string HsCode, decimal CustomsValue, decimal TaxableValue, decimal Duty, decimal Vat);

    public record DutyCalculationResponse(
        DeclarantType DeclarantType,
        decimal TotalCustomsValue,
        decimal ExemptAmount,
        decimal TaxableValue,
        decimal TotalDuty,
        decimal TotalVat,
        decimal TotalPayable,
        IReadOnlyList<DutyLineDto> Lines);

    public record HsCodeDto(Guid Id, string Code, string Description, decimal DutyRate, decimal VatRate);
    public record WarehouseDto(Guid Id, string Name, string Address);

}
