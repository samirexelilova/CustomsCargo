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
    public class DeclarationService(
     IDeclarationRepository declarations,
     IHsCodeRepository hsCodes,
     IWarehouseRepository warehouses,
     IUserRepository users,
     IUnitOfWork uow,
     CustomsDutyCalculator calculator,
     ICurrentUser currentUser,
     TimeProvider clock,
     ILogger<DeclarationService> logger) : IDeclarationService
    {
        private const int MaxItems = 100;

        public Task<PagedResult<DeclarationListItemDto>> GetPagedAsync(DeclarationQuery query, CancellationToken ct)
        {
            var safe = query with { Page = Math.Max(1, query.Page), PageSize = Math.Clamp(query.PageSize, 1, 100) };
            return declarations.GetPagedAsync(safe, BuildScope(), ct);
        }

        public async Task<DeclarationDetailDto> GetByIdAsync(Guid id, CancellationToken ct)
            => await declarations.GetDetailAsync(id, BuildScope(), ct)
               ?? throw new NotFoundException("Bəyannamə tapılmadı.");

        public async Task<DeclarationDetailDto> CreateAsync(SaveDeclarationRequest req, CancellationToken ct)
        {
            var userId = RequireDeclarant();
            var user = await users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAppException("İstifadəçi tapılmadı.");
            var type = user.DeclarantType ?? throw new ForbiddenException("Deklarant növü təyin olunmayıb.");

            var found = await ValidateAndResolveAsync(req, ct);

            var number = $"AZ-{clock.GetUtcNow().Year}-{await declarations.GetNextNumberAsync(ct):D6}";
            var declaration = Declaration.CreateDraft(number, userId, type, req.WarehouseId, clock.GetUtcNow());
            declaration.ReplaceItems(BuildItems(req, found), req.WarehouseId);
            declaration.RecalculateDuty(calculator);

            declarations.Add(declaration);
            await uow.SaveChangesAsync(ct);

            logger.LogInformation(
                "Bəyannamə yaradıldı: {Number}, istifadəçi {UserId}, rüsum {Duty}, ƏDV {Vat}",
                declaration.Number, userId, declaration.TotalDuty, declaration.TotalVat);

            return await GetDetailOrThrow(declaration.Id, ct);
        }

        public async Task<DeclarationDetailDto> UpdateAsync(Guid id, SaveDeclarationRequest req, CancellationToken ct)
        {
            var userId = RequireDeclarant();
            var declaration = await declarations.GetForUpdateAsync(id, ct);

            if (declaration is null || declaration.DeclarantUserId != userId)
                throw new NotFoundException("Bəyannamə tapılmadı.");

            declaration.EnsureEditable();
            var found = await ValidateAndResolveAsync(req, ct);

            declaration.ReplaceItems(BuildItems(req, found), req.WarehouseId);
            declaration.RecalculateDuty(calculator);
            await uow.SaveChangesAsync(ct);

            logger.LogInformation("Bəyannamə yeniləndi: {Number}, istifadəçi {UserId}", declaration.Number, userId);

            return await GetDetailOrThrow(id, ct);
        }

        public async Task<DeclarationDetailDto> ChangeStatusAsync(Guid id, ChangeStatusRequest req, CancellationToken ct)
        {
            var role = currentUser.Role ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
            var userId = currentUser.UserId ?? throw new UnauthorizedAppException("Giriş tələb olunur.");

            var declaration = await declarations.GetForUpdateAsync(id, ct);

            var visible = declaration is not null &&
                (role == UserRole.Declarant
                    ? declaration.DeclarantUserId == userId
                    : declaration.Status != DeclarationStatus.Draft);
            if (!visible) throw new NotFoundException("Bəyannamə tapılmadı.");

            var from = declaration!.Status;
            var to = req.TargetStatus;

            if (!DeclarationStateMachine.IsValidTransition(from, to))
                throw new ConflictException($"Statusu {from} -> {to} keçirmək mümkün deyil.");

            if (!DeclarationStateMachine.IsAllowed(from, to, role))
                throw new ForbiddenException($"{role} rolu bu keçidi ({from} -> {to}) edə bilməz.");

            var errors = new ErrorCollector();
            if (to == DeclarationStatus.Rejected && string.IsNullOrWhiteSpace(req.Reason))
                errors.Add("reason", "Rədd üçün səbəb yazılmalıdır.");
            if (req.Reason?.Length > 500)
                errors.Add("reason", "Səbəb 500 simvoldan çox olmasın.");
            if (to == DeclarationStatus.Submitted && declaration.Items.Count == 0)
                errors.Add("items", "Mal olmayan bəyannamə göndərilə bilməz.");
            errors.ThrowIfAny();

            if (to == DeclarationStatus.Submitted)
                declaration.RecalculateDuty(calculator);

            declaration.ChangeStatus(to, userId, req.Reason?.Trim(), clock.GetUtcNow());
            await uow.SaveChangesAsync(ct);

            logger.LogInformation(
                "Bəyannamə {Number} statusu dəyişdi: {From} -> {To}, istifadəçi {UserId}, rol {Role}",
                declaration.Number, from, to, userId, role);

            return await GetDetailOrThrow(id, ct);
        }

        private DeclarationScope BuildScope()
        {
            var role = currentUser.Role ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
            return role == UserRole.Declarant
                ? new DeclarationScope(currentUser.UserId, ExcludeDrafts: false)
                : new DeclarationScope(null, ExcludeDrafts: true);
        }

        private Guid RequireDeclarant()
        {
            if (currentUser.Role != UserRole.Declarant)
                throw new ForbiddenException("Bu əməliyyat yalnız Deklarant üçündür.");
            return currentUser.UserId ?? throw new UnauthorizedAppException("Giriş tələb olunur.");
        }

        private async Task<DeclarationDetailDto> GetDetailOrThrow(Guid id, CancellationToken ct)
            => await declarations.GetDetailAsync(id, DeclarationScope.All, ct)
               ?? throw new NotFoundException("Bəyannamə tapılmadı.");

        private async Task<Dictionary<string, HsCode>> ValidateAndResolveAsync(SaveDeclarationRequest req, CancellationToken ct)
        {
            var errors = new ErrorCollector();

            if (req.WarehouseId == Guid.Empty)
                errors.Add("warehouseId", "Anbar seçilməlidir.");

            if (req.Items is null || req.Items.Count == 0)
                errors.Add("items", "Ən azı bir mal əlavə edilməlidir.");
            else if (req.Items.Count > MaxItems)
                errors.Add("items", $"Bir bəyannamədə ən çox {MaxItems} mal ola bilər.");
            else
                for (var i = 0; i < req.Items.Count; i++)
                {
                    var it = req.Items[i];
                    RequestRules.ValidateLine(errors, i, it.HsCode, it.Quantity, it.UnitPrice);
                    if (string.IsNullOrWhiteSpace(it.Description) || it.Description.Length > 500)
                        errors.Add($"items[{i}].description", "Təsvir 1-500 simvol olmalıdır.");
                    if (it.WeightKg is < 0 or > 1_000_000)
                        errors.Add($"items[{i}].weightKg", "Çəki yanlışdır.");
                }
            errors.ThrowIfAny();

            if (!await warehouses.ExistsAsync(req.WarehouseId, ct))
            {
                errors.Add("warehouseId", "Belə anbar yoxdur.");
                errors.ThrowIfAny();
            }

            var codes = req.Items.Select(i => i.HsCode).Distinct().ToList();
            var found = (await hsCodes.GetByCodesAsync(codes, ct)).ToDictionary(h => h.Code);
            foreach (var code in codes.Where(c => !found.ContainsKey(c)))
                errors.Add("items", $"HS kodu tapılmadı: {code}");
            errors.ThrowIfAny();

            return found;
        }

        private static List<DeclarationItem> BuildItems(SaveDeclarationRequest req, Dictionary<string, HsCode> found)
            => req.Items.Select(i => new DeclarationItem
            {
                HsCode = found[i.HsCode],
                HsCodeId = found[i.HsCode].Id,
                Description = i.Description.Trim(),
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                WeightKg = i.WeightKg
            }).ToList();
    }

}
