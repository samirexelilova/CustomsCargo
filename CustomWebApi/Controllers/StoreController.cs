using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomWebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/store")]
    public class StoreController(IStoreService service) : ControllerBase
    {
        [HttpGet("products")]
        public async Task<ActionResult<IReadOnlyList<StoreProductDto>>> Products(CancellationToken ct)
            => Ok(await service.ListProductsAsync(ct));

        [HttpGet("cargo-companies")]
        public async Task<ActionResult<IReadOnlyList<CargoCompanyDto>>> CargoCompanies(CancellationToken ct)
            => Ok(await service.ListCargoCompaniesAsync(ct));

        [HttpPost("orders"), Authorize(Policy = Policies.DeclarantOnly)]
        public async Task<ActionResult<StoreOrderDto>> CreateOrder(CreateStoreOrderRequest request, CancellationToken ct)
            => StatusCode(StatusCodes.Status201Created, await service.CreateOrderAsync(request, ct));

        [HttpGet("orders"), Authorize(Policy = Policies.DeclarantOnly)]
        public async Task<ActionResult<IReadOnlyList<StoreOrderDto>>> MyOrders(CancellationToken ct)
            => Ok(await service.ListMyOrdersAsync(ct));
    }


}
