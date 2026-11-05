using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class StoreOrder : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<StoreOrderItem> Items { get; set; } = new List<StoreOrderItem>();
    }
}
