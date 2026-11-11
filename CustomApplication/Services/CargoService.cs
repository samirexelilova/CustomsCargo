using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomDomain.Enums;
using CustomDomain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Services
{
    public class CargoService(
        ICargoParcelRepository parcels,
        IDeclarationRepository declarations,
        IUserRepository users,
        IUnitOfWork uow,
        CustomsDutyCalculator calculator,
        ICurrentUser currentUser,
        TimeProvider clock,
        ILogger<CargoService> logger) : ICargoService
    {
        public Task<PagedResult<CargoParcelListItemDto>> GetPagedAsync(CargoParcelQuery query, CancellationToken ct)
        {
            var safe = query with { Page = Math.Max(1, query.Page), PageSize = Math.Clamp(query.PageSize, 1, 100) };
            return parcels.GetPagedAsync(safe, OwnerFilter(), ct);
        }

        public async Task<CargoParcelDetailDto> GetByIdAsync(Guid id, CancellationToken ct)
            => await parcels.GetDetailAsync(id, OwnerFilter(), ct)
               ?? throw new NotFoundException("Bağlama tapılmadı.");

        public async Task<DeclarationDetailDto> DeclareAsync(Guid id, CancellationToken ct)
        {
            if (currentUser.Role != UserRole.Declarant)
                throw new ForbiddenException("Bağlamanı yalnız Deklarant bəyan edə bilər.");
            var userId = currentUser.UserId ?? throw new UnauthorizedAppException("Giriş tələb olunur.");

            var parcel = await parcels.GetForDeclareAsync(id, ct);

            if (parcel is null || parcel.OwnerUserId != userId)
                throw new NotFoundException("Bağlama tapılmadı.");

            if (parcel.Status != CargoParcelStatus.Arrived)
                throw new ConflictException("Bu bağlama artıq bəyan edilib.");

            var user = await users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAppException("İstifadəçi tapılmadı.");
            var type = user.DeclarantType ?? throw new ForbiddenException("Deklarant növü təyin olunmayıb.");

            var now = clock.GetUtcNow();
            var number = $"AZ-{now.Year}-{await declarations.GetNextNumberAsync(ct):D6}";

            var declaration = Declaration.CreateDraft(number, userId, type, parcel.WarehouseId, now);

            var items = parcel.Order.Items.Select(i => new DeclarationItem
            {
                HsCode = i.Product.HsCode,
                HsCodeId = i.Product.HsCodeId,
                Description = $"{i.Product.Name} ({i.Product.StoreName})",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                WeightKg = i.Product.WeightKg * i.Quantity
            }).ToList();

            declaration.ReplaceItems(items, parcel.WarehouseId);
            declaration.RecalculateDuty(calculator);

            parcel.MarkDeclared(declaration.Id, now);  
            declarations.Add(declaration);
            await uow.SaveChangesAsync(ct);             

            logger.LogInformation(
                "Bağlama bəyan edildi: {TrackingNumber} -> bəyannamə {DeclarationNumber}, istifadəçi {UserId}",
                parcel.TrackingNumber, number, userId);

            return await declarations.GetDetailAsync(declaration.Id, DeclarationScope.All, ct)
                   ?? throw new NotFoundException("Bəyannamə tapılmadı.");
        }

        private Guid? OwnerFilter()
        {
            var role = currentUser.Role ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
            if (role != UserRole.Declarant) return null;
            return currentUser.UserId ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
        }
    }
}
