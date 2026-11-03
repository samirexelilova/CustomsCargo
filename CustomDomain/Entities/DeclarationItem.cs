using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class DeclarationItem : BaseEntity
    {
        public Guid DeclarationId { get; set; }
        public Declaration Declaration { get; set; } = null!;

        public Guid HsCodeId { get; set; }
        public HsCode HsCode { get; set; } = null!;

        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }   // USD
        public decimal WeightKg { get; set; }

        // Hesablanmış dəyərlər (bəyannamə saxlananda yazılır)
        public decimal DutyAmount { get; set; }
        public decimal VatAmount { get; set; }

        //Gömrük dəyəri = say x vahid qiymət 
        public decimal CustomsValue => Quantity * UnitPrice;
    }
}

