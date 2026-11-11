using CustomDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class DeclarationStatusHistory : BaseEntity
    {
        public Guid DeclarationId { get; set; }
        public Declaration Declaration { get; set; } = null!;

        public DeclarationStatus? FromStatus { get; set; }
        public DeclarationStatus ToStatus { get; set; }

        public Guid ChangedByUserId { get; set; }
        public User ChangedBy { get; set; } = null!;

        public DateTimeOffset ChangedAt { get; set; }
        public string? Reason { get; set; }
    }

}
