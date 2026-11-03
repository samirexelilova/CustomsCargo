using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/duty")]
    public class DutyController(IDutyService service) : ControllerBase
    {
        //Rüsum kalkulyatoru bəyannamə yaratmadan hesablayır
        [HttpPost("calculate")]
        public async Task<ActionResult<DutyCalculationResponse>> Calculate(CalculateDutyRequest request, CancellationToken ct)
            => Ok(await service.CalculateAsync(request, ct));
    }

}
