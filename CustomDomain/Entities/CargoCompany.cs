using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class CargoCompany : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
    }
}
