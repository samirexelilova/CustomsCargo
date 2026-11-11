using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class StoreOrderItem : BaseEntity
    {
        public Guid StoreOrderId { get; set; }
        public StoreOrder Order { get; set; } = null!;

        public Guid StoreProductId { get; set; }
        public StoreProduct Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }   
    }

}
