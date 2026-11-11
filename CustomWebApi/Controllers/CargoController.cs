using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomWebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cargo/parcels")]
    public class CargoController(ICargoService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<CargoParcelListItemDto>>> GetList(
            [FromQuery] CargoParcelQuery query, CancellationToken ct)
            => Ok(await service.GetPagedAsync(query, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CargoParcelDetailDto>> GetById(Guid id, CancellationToken ct)
            => Ok(await service.GetByIdAsync(id, ct));

        [HttpPost("{id:guid}/declare"), Authorize(Policy = Policies.DeclarantOnly)]
        public async Task<ActionResult<DeclarationDetailDto>> Declare(Guid id, CancellationToken ct)
            => StatusCode(StatusCodes.Status201Created, await service.DeclareAsync(id, ct));
    }

}
