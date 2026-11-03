using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomWebApi.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api")]
    public class ReferenceController(IReferenceDataService service) : ControllerBase
    {
        [HttpGet("hs-codes")]
        public async Task<ActionResult<IReadOnlyList<HsCodeDto>>> HsCodes([FromQuery] string? search, CancellationToken ct)
            => Ok(await service.SearchHsCodesAsync(search, ct));

        [HttpGet("warehouses")]
        public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> Warehouses(CancellationToken ct)
            => Ok(await service.ListWarehousesAsync(ct));
    }

}
