using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomApplication.Common
{
    public sealed class ErrorCollector
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public void Add(string field, string message)
        {
            if (!_errors.TryGetValue(field, out var list))
                _errors[field] = list = new List<string>();
            list.Add(message);
        }

        public void ThrowIfAny()
        {
            if (_errors.Count > 0)
                throw new RequestValidationException(_errors.ToDictionary(k => k.Key, k => k.Value.ToArray()));
        }
    }

    //Ortaq yoxlama qaydaları
    public static class RequestRules
    {
        private static readonly Regex HsCodePattern = new(@"^\d{6,10}$", RegexOptions.Compiled);

        public static bool IsValidHsCode(string? code) => code is not null && HsCodePattern.IsMatch(code);

        public static void ValidateLine(ErrorCollector e, int index, string? hsCode, int quantity, decimal unitPrice)
        {
            if (!IsValidHsCode(hsCode))
                e.Add($"items[{index}].hsCode", "HS kodu 6-10 rəqəmdən ibarət olmalıdır.");
            if (quantity is < 1 or > 1_000_000)
                e.Add($"items[{index}].quantity", "Say 1 ilə 1 000 000 arasında olmalıdır.");
            if (unitPrice is < 0 or > 10_000_000)
                e.Add($"items[{index}].unitPrice", "Vahid qiymət 0 ilə 10 000 000 arasında olmalıdır.");
        }
    }

}
