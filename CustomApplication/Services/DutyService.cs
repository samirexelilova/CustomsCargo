using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Services
{
    public class DutyService(IHsCodeRepository hsCodes, CustomsDutyCalculator calculator) : IDutyService
    {
        public async Task<DutyCalculationResponse> CalculateAsync(CalculateDutyRequest req, CancellationToken ct)
        {
            var errors = new ErrorCollector();
            if (!Enum.IsDefined(req.DeclarantType))
                errors.Add("declarantType", "Deklarant növü yanlışdır.");

            if (req.Items is null || req.Items.Count == 0 || req.Items.Count > 100)
                errors.Add("items", "1-100 arası mal göndərilməlidir.");
            else
                for (var i = 0; i < req.Items.Count; i++)
                    RequestRules.ValidateLine(errors, i, req.Items[i].HsCode, req.Items[i].Quantity, req.Items[i].UnitPrice);
            errors.ThrowIfAny();

            var codes = req.Items.Select(i => i.HsCode).Distinct().ToList();
            var found = (await hsCodes.GetByCodesAsync(codes, ct)).ToDictionary(h => h.Code);

            foreach (var code in codes.Where(c => !found.ContainsKey(c)))
                errors.Add("items", $"HS kodu tapılmadı: {code}");
            errors.ThrowIfAny();

            var inputs = req.Items
                .Select(i => new DutyLineInput(i.Quantity * i.UnitPrice, found[i.HsCode].DutyRate, found[i.HsCode].VatRate))
                .ToList();

            var r = calculator.Calculate(req.DeclarantType, inputs);

            var lines = r.Lines
                .Select((l, idx) => new DutyLineDto(req.Items[idx].HsCode, l.CustomsValue, l.TaxableValue, l.Duty, l.Vat))
                .ToList();

            return new DutyCalculationResponse(
                req.DeclarantType, r.TotalCustomsValue, r.ExemptAmount, r.TaxableValue,
                r.TotalDuty, r.TotalVat, r.TotalPayable, lines);
        }
    }
}
