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
    public class HsCodeRepository(AppDbContext db) : IHsCodeRepository
    {
       
        public async Task<IReadOnlyList<HsCode>> GetByCodesAsync(IReadOnlyCollection<string> codes, CancellationToken ct)
            => await db.HsCodes.Where(h => codes.Contains(h.Code)).ToListAsync(ct);

        public async Task<IReadOnlyList<HsCodeDto>> SearchAsync(string? search, CancellationToken ct)
        {
            var q = db.HsCodes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(h => h.Code.StartsWith(s) || h.Description.Contains(s));
            }
            return await q.OrderBy(h => h.Code).Take(50)
                .Select(h => new HsCodeDto(h.Id, h.Code, h.Description, h.DutyRate, h.VatRate))
                .ToListAsync(ct);
        }
    }

    public class WarehouseRepository(AppDbContext db) : IWarehouseRepository
    {
        public Task<bool> ExistsAsync(Guid id, CancellationToken ct) => db.Warehouses.AnyAsync(w => w.Id == id, ct);

        public async Task<IReadOnlyList<WarehouseDto>> ListAsync(CancellationToken ct)
            => await db.Warehouses.AsNoTracking().OrderBy(w => w.Name)
                .Select(w => new WarehouseDto(w.Id, w.Name, w.Address))
                .ToListAsync(ct);
    }

}
