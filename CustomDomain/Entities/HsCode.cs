using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    // Mal nomenklaturu. Dərəcələr faiz kəsridir: 0.15 = 15%
    public class HsCode : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DutyRate { get; set; }
        public decimal VatRate { get; set; }
    }
}
