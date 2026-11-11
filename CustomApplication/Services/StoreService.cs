using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomDomain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Services
{
 
    public class StoreService(
        IStoreRepository store,
        ICargoCompanyRepository cargoCompanies,
        IUnitOfWork uow,
        ICurrentUser currentUser,
        TimeProvider clock,
        ILogger<StoreService> logger) : IStoreService
    {
        public Task<IReadOnlyList<StoreProductDto>> ListProductsAsync(CancellationToken ct)
            => store.ListProductsAsync(ct);

        public Task<IReadOnlyList<CargoCompanyDto>> ListCargoCompaniesAsync(CancellationToken ct)
            => cargoCompanies.ListActiveAsync(ct);

        public Task<IReadOnlyList<StoreOrderDto>> ListMyOrdersAsync(CancellationToken ct)
            => store.GetOrdersAsync(RequireDeclarant(), ct);

        public async Task<StoreOrderDto> CreateOrderAsync(CreateStoreOrderRequest req, CancellationToken ct)
        {
            var userId = RequireDeclarant();

            var errors = new ErrorCollector();
            if (req.Items is null || req.Items.Count is < 1 or > 20)
                errors.Add("items", "1-20 məhsul seçilməlidir.");
            else
            {
                for (var i = 0; i < req.Items.Count; i++)
                    if (req.Items[i].Quantity is < 1 or > 50)
                        errors.Add($"items[{i}].quantity", "Say 1-50 arası olmalıdır.");

                if (req.Items.Select(i => i.ProductId).Distinct().Count() != req.Items.Count)
                    errors.Add("items", "Eyni məhsul təkrar əlavə olunub.");
            }
            errors.ThrowIfAny();

            var ids = req.Items.Select(i => i.ProductId).ToList();
            var products = (await store.GetProductsByIdsAsync(ids, ct)).ToDictionary(p => p.Id);
            foreach (var id in ids.Where(id => !products.ContainsKey(id)))
                errors.Add("items", $"Məhsul tapılmadı: {id}");

            var active = await cargoCompanies.ListActiveAsync(ct);
            CargoCompanyDto? company = null;
            if (req.CargoCompanyId is { } companyId)
            {
                company = active.FirstOrDefault(c => c.Id == companyId);
                if (company is null)
                    errors.Add("cargoCompanyId", "Seçilmiş kargo şirkəti mövcud deyil və ya aktiv deyil.");
            }
            else if (active.Count == 1)
                company = active[0];
            else
                errors.Add("cargoCompanyId", active.Count == 0 ? "Aktiv kargo şirkəti yoxdur." : "Kargo şirkətini seçin.");

            errors.ThrowIfAny();

            var now = clock.GetUtcNow();
            var order = new StoreOrder
            {
                OrderNumber = NewCode("ORD"),
                UserId = userId,
                CreatedAt = now
            };
            foreach (var item in req.Items)
                order.Items.Add(new StoreOrderItem
                {
                    StoreProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = products[item.ProductId].PriceUsd  
                });
            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

            var parcel = new CargoParcel
            {
                TrackingNumber = NewCode("TRK"),
                StoreOrderId = order.Id,
                OwnerUserId = userId,
                CargoCompanyId = company!.Id,
                WarehouseId = company.WarehouseId,
                Status = CargoParcelStatus.Arrived,
                ArrivedAt = now
            };
            order.Parcel = parcel;

            store.AddOrder(order);
            await uow.SaveChangesAsync(ct);

            logger.LogInformation(
                "Sifariş yaradıldı: {OrderNumber}, istifadəçi {UserId}, məhsul sayı {ItemCount}, məbləğ {Total}, kargo {Cargo}, bağlama {TrackingNumber}",
                order.OrderNumber, userId, order.Items.Count, order.TotalAmount, company.Name, parcel.TrackingNumber);

            var items = order.Items.Select(i =>
            {
                var p = products[i.StoreProductId];
                return new StoreOrderItemDto(p.Id, p.Name, p.StoreName, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice);
            }).ToList();

            return new StoreOrderDto(order.Id, order.OrderNumber, now, order.TotalAmount,
                items, parcel.Id, parcel.TrackingNumber, company.Name);
        }

        private Guid RequireDeclarant()
        {
            if (currentUser.Role != UserRole.Declarant)
                throw new ForbiddenException("Sifariş yalnız Deklarant üçündür.");
            return currentUser.UserId ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
        }

        private static string NewCode(string prefix)
            => $"{prefix}-{Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()}";
    }

}
