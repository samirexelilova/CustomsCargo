using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Services
{
    public class ReferenceDataService(IHsCodeRepository hsCodes, IWarehouseRepository warehouses) : IReferenceDataService
    {
        public Task<IReadOnlyList<HsCodeDto>> SearchHsCodesAsync(string? search, CancellationToken ct)
            => hsCodes.SearchAsync(search, ct);

        public Task<IReadOnlyList<WarehouseDto>> ListWarehousesAsync(CancellationToken ct)
            => warehouses.ListAsync(ct);
    }

}
