using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Services
{
    public record DutyLineInput(decimal CustomsValue, decimal DutyRate, decimal VatRate);

    public record DutyLineResult(decimal CustomsValue, decimal TaxableValue, decimal Duty, decimal Vat);

    public record DutyCalculationResult(
        decimal TotalCustomsValue,
        decimal ExemptAmount,
        decimal TaxableValue,
        decimal TotalDuty,
        decimal TotalVat,
        IReadOnlyList<DutyLineResult> Lines)
    {
        public decimal TotalPayable => TotalDuty + TotalVat;
    }

}
