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
    public class StoreRepository(AppDbContext db) : IStoreRepository
    {
        public async Task<IReadOnlyList<StoreProductDto>> ListProductsAsync(CancellationToken ct)
            => await db.StoreProducts.AsNoTracking()
                .OrderBy(p => p.StoreName).ThenBy(p => p.Name)
                .Select(p => new StoreProductDto(p.Id, p.StoreName, p.Name, p.PriceUsd, p.WeightKg, p.HsCode.Code))
                .ToListAsync(ct);

        public async Task<IReadOnlyList<StoreProduct>> GetProductsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
            => await db.StoreProducts.AsNoTracking().Where(p => ids.Contains(p.Id)).ToListAsync(ct);

        public async Task<IReadOnlyList<StoreOrderDto>> GetOrdersAsync(Guid userId, CancellationToken ct)
            => await db.StoreOrders.AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Take(50)
                .Select(o => new StoreOrderDto(
                    o.Id, o.OrderNumber, o.CreatedAt, o.TotalAmount,
                    o.Items.Select(i => new StoreOrderItemDto(
                        i.StoreProductId, i.Product.Name, i.Product.StoreName,
                        i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList()))
                .ToListAsync(ct);

        public void AddOrder(StoreOrder order) => db.StoreOrders.Add(order);
    }

}
