using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomDomain.Enums;
using CustomInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Repositories
{
    public class DeclarationRepository(AppDbContext db) : IDeclarationRepository
    {
        private IQueryable<Declaration> Scoped(DeclarationScope scope)
        {
            var q = db.Declarations.AsNoTracking().AsQueryable();
            if (scope.OwnerId is { } owner) q = q.Where(d => d.DeclarantUserId == owner);
            if (scope.ExcludeDrafts) q = q.Where(d => d.Status != DeclarationStatus.Draft);
            return q;
        }

        public async Task<PagedResult<DeclarationListItemDto>> GetPagedAsync(
            DeclarationQuery q, DeclarationScope scope, CancellationToken ct)
        {
            var query = Scoped(scope);

            if (q.Status is not null)
                query = query.Where(d => d.Status == q.Status);

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                query = query.Where(d => d.Number.Contains(s) || d.Declarant.FullName.Contains(s));
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(d => new DeclarationListItemDto(
                    d.Id, d.Number, d.Status, d.DeclarantType, d.Declarant.FullName,
                    d.TotalDuty + d.TotalVat, d.CreatedAt, d.Items.Count))
                .ToListAsync(ct);

            return new PagedResult<DeclarationListItemDto>(items, q.Page, q.PageSize, total);
        }

        public Task<DeclarationDetailDto?> GetDetailAsync(Guid id, DeclarationScope scope, CancellationToken ct)
            => Scoped(scope)
                .Where(d => d.Id == id)
                .Select(d => new DeclarationDetailDto(
                    d.Id, d.Number, d.Status, d.DeclarantType, d.Declarant.FullName,
                    d.WarehouseId, d.Warehouse.Name,
                    d.TotalCustomsValue, d.ExemptAmount, d.TotalDuty, d.TotalVat, d.TotalDuty + d.TotalVat,
                    d.CreatedAt, d.SubmittedAt,
                    d.Items.Select(i => new DeclarationItemDto(
                        i.Id, i.HsCode.Code, i.Description, i.Quantity, i.UnitPrice,
                        i.Quantity * i.UnitPrice, i.WeightKg, i.DutyAmount, i.VatAmount)).ToList(),
                    d.History.OrderBy(h => h.ChangedAt).Select(h => new StatusHistoryDto(
                        h.FromStatus, h.ToStatus, h.ChangedBy.FullName, h.ChangedAt, h.Reason)).ToList()))
                .FirstOrDefaultAsync(ct);

        public Task<Declaration?> GetForUpdateAsync(Guid id, CancellationToken ct)
            => db.Declarations
                .Include(d => d.Items).ThenInclude(i => i.HsCode)
                .FirstOrDefaultAsync(d => d.Id == id, ct);

        public void Add(Declaration declaration) => db.Declarations.Add(declaration);

        public async Task<int> GetNextNumberAsync(CancellationToken ct)
        {
            using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT NEXT VALUE FOR DeclarationNumbers";

            if (command.Connection.State != System.Data.ConnectionState.Open)
                await command.Connection.OpenAsync(ct);

            var result = await command.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }
    }

}
