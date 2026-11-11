using CustomDomain.Enums;
using CustomDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class CargoParcel : BaseEntity
    {
        public string TrackingNumber { get; set; } = string.Empty;

        public Guid StoreOrderId { get; set; }
        public StoreOrder Order { get; set; } = null!;

        public Guid OwnerUserId { get; set; }
        public User Owner { get; set; } = null!;

        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public Guid? CargoCompanyId { get; set; }
        public CargoCompany? CargoCompany { get; set; }

        public CargoParcelStatus Status { get; set; } = CargoParcelStatus.Arrived;
        public DateTimeOffset ArrivedAt { get; set; }

        public Guid? DeclarationId { get; set; }
        public Declaration? Declaration { get; set; }
        public DateTimeOffset? DeclaredAt { get; set; }

        public void MarkDeclared(Guid declarationId, DateTimeOffset now)
        {
            if (Status != CargoParcelStatus.Arrived)
                throw new DomainException("Bu bağlama artıq bəyan edilib.");

            DeclarationId = declarationId;
            DeclaredAt = now;
            Status = CargoParcelStatus.Declared;
        }
    }
}
