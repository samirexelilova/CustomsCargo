using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Services
{
  
    // Rüsum və ƏDV kalkulyatoru
    // Qaydalar:
    // - Hüquqi şəxs: bütün gömrük dəyəri vergiyə cəlb olunur.
    //  - Fiziki şəxs: ümumi dəyərin 300 USD-lik hissəsi güzəştlidir; yalnız artıq hissə vergilənir.
    //   Artıq hissə mallar arasında dəyərinə mütənasib bölünür.
    //  - Rüsum = vergiyə cəlb olunan dəyər x rüsum dərəcəsi
    //  - ƏDV   = (vergiyə cəlb olunan dəyər + rüsum) x ƏDV dərəcəsi
    //  - Bütün məbləğlər USD-dir.
  
    public sealed class CustomsDutyCalculator
    {
        public DutyCalculationResult Calculate(DeclarantType declarantType, IReadOnlyList<DutyLineInput> lines)
        {
            var total = lines.Sum(l => l.CustomsValue);

            decimal exempt = 0m;
            decimal taxableRatio = 1m;

            if (declarantType == DeclarantType.Individual)
            {
                exempt = Math.Min(total, CustomsRules.IndividualExemptionLimitUsd);
                taxableRatio = total == 0m ? 0m : (total - exempt) / total;
            }

            var results = new List<DutyLineResult>(lines.Count);
            foreach (var line in lines)
            {
                var taxable = Round(line.CustomsValue * taxableRatio);
                var duty = Round(taxable * line.DutyRate);
                var vat = Round((taxable + duty) * line.VatRate);
                results.Add(new DutyLineResult(line.CustomsValue, taxable, duty, vat));
            }

            return new DutyCalculationResult(
                TotalCustomsValue: Round(total),
                ExemptAmount: Round(exempt),
                TaxableValue: results.Sum(r => r.TaxableValue),
                TotalDuty: results.Sum(r => r.Duty),
                TotalVat: results.Sum(r => r.Vat),
                Lines: results);
        }

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
