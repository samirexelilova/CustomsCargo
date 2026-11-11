using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInfrastructure.Repositories
{
    public class CargoParcelRepository(AppDbContext db) : ICargoParcelRepository
    {
        private IQueryable<CargoParcel> Scoped(Guid? ownerId)
        {
            var q = db.CargoParcels.AsNoTracking().AsQueryable();
            if (ownerId is { } owner) q = q.Where(p => p.OwnerUserId == owner);
            return q;
        }

        public async Task<PagedResult<CargoParcelListItemDto>> GetPagedAsync(CargoParcelQuery query, Guid? ownerId, CancellationToken ct)
        {
            var q = Scoped(ownerId);
            if (query.Status is not null) q = q.Where(p => p.Status == query.Status);

            var total = await q.CountAsync(ct);
            var items = await q
                .OrderByDescending(p => p.ArrivedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new CargoParcelListItemDto(
                    p.Id, p.TrackingNumber, p.Status, p.ArrivedAt, p.Warehouse.Name, p.Owner.FullName,
                    p.Order.TotalAmount, p.Order.Items.Count, p.DeclarationId,
                    p.CargoCompany != null ? p.CargoCompany.Name : null))
                .ToListAsync(ct);

            return new PagedResult<CargoParcelListItemDto>(items, query.Page, query.PageSize, total);
        }

        public Task<CargoParcelDetailDto?> GetDetailAsync(Guid id, Guid? ownerId, CancellationToken ct)
            => Scoped(ownerId)
                .Where(p => p.Id == id)
                .Select(p => new CargoParcelDetailDto(
                    p.Id, p.TrackingNumber, p.Order.OrderNumber, p.Status, p.ArrivedAt,
                    p.Warehouse.Name, p.Owner.FullName, p.Order.TotalAmount,
                    p.DeclarationId, p.DeclaredAt,
                    p.Order.Items.Select(i => new CargoParcelItemDto(
                        i.Product.Name, i.Product.StoreName, i.Product.HsCode.Code,
                        i.Quantity, i.UnitPrice, i.Product.WeightKg * i.Quantity)).ToList(),
                    p.CargoCompany != null ? p.CargoCompany.Name : null))
                .FirstOrDefaultAsync(ct);

        public Task<CargoParcel?> GetForDeclareAsync(Guid id, CancellationToken ct)
            => db.CargoParcels
                .Include(p => p.Order).ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product).ThenInclude(pr => pr.HsCode)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public class CargoCompanyRepository(AppDbContext db) : ICargoCompanyRepository
    {
        public async Task<IReadOnlyList<CargoCompanyDto>> ListActiveAsync(CancellationToken ct)
            => await db.CargoCompanies.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CargoCompanyDto(c.Id, c.Name, c.WarehouseId, c.Warehouse.Name))
                .ToListAsync(ct);
    }

}
