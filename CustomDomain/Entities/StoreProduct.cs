using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class StoreProduct : BaseEntity
    {
        public string StoreName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal PriceUsd { get; set; }
        public decimal WeightKg { get; set; }

        public Guid HsCodeId { get; set; }
        public HsCode HsCode { get; set; } = null!;
    }

}
