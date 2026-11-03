using CustomDomain.Enums;
using CustomDomain.Exceptions;
using CustomDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDomain.Entities
{
    public class Declaration : BaseEntity
    {
        public string Number { get; set; } = string.Empty;
        public DeclarationStatus Status { get; set; } = DeclarationStatus.Draft;

        public Guid DeclarantUserId { get; set; }
        public User Declarant { get; set; } = null!;

        public DeclarantType DeclarantType { get; set; }

        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public decimal TotalCustomsValue { get; set; }
        public decimal ExemptAmount { get; set; }
        public decimal TotalDuty { get; set; }
        public decimal TotalVat { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }

        public ICollection<DeclarationItem> Items { get; set; } = new List<DeclarationItem>();
        public ICollection<DeclarationStatusHistory> History { get; set; } = new List<DeclarationStatusHistory>();

        public static Declaration CreateDraft(
            string number, Guid declarantUserId, DeclarantType type, Guid warehouseId, DateTimeOffset now)
        {
            var d = new Declaration
            {
                Number = number,
                DeclarantUserId = declarantUserId,
                DeclarantType = type,
                WarehouseId = warehouseId,
                Status = DeclarationStatus.Draft,
                CreatedAt = now
            };
            d.History.Add(new DeclarationStatusHistory
            {
                DeclarationId = d.Id,
                FromStatus = null,
                ToStatus = DeclarationStatus.Draft,
                ChangedByUserId = declarantUserId,
                ChangedAt = now
            });
            return d;
        }

        public void EnsureEditable()
        {
            if (Status != DeclarationStatus.Draft)
                throw new DomainException("Yalnız qaralama (Draft) statusunda olan bəyannaməni redaktə etmək olar");
        }

        public void ReplaceItems(IEnumerable<DeclarationItem> items, Guid warehouseId)
        {
            EnsureEditable();
            WarehouseId = warehouseId;
            Items.Clear();
            foreach (var item in items) Items.Add(item);
        }

        public void RecalculateDuty(CustomsDutyCalculator calculator)
        {
            var items = Items.ToList();
            var inputs = items
                .Select(i => new DutyLineInput(i.CustomsValue, i.HsCode.DutyRate, i.HsCode.VatRate))
                .ToList();

            var result = calculator.Calculate(DeclarantType, inputs);

            for (var i = 0; i < items.Count; i++)
            {
                items[i].DutyAmount = result.Lines[i].Duty;
                items[i].VatAmount = result.Lines[i].Vat;
            }
            TotalCustomsValue = result.TotalCustomsValue;
            ExemptAmount = result.ExemptAmount;
            TotalDuty = result.TotalDuty;
            TotalVat = result.TotalVat;
        }

        public void ChangeStatus(DeclarationStatus to, Guid userId, string? reason, DateTimeOffset now)
        {
            History.Add(new DeclarationStatusHistory
            {
                DeclarationId = Id,
                FromStatus = Status,
                ToStatus = to,
                ChangedByUserId = userId,
                ChangedAt = now,
                Reason = reason
            });
            Status = to;
            if (to == DeclarationStatus.Submitted) SubmittedAt = now;
        }
    }
}
