using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomApplication.Interfaces;
using CustomDomain.Entities;
using CustomWebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomWebApi.Controllers
{
    // Controller-də məntiq yoxdur: yalnız HTTP <-> servis
    [ApiController]
    [Authorize]
    [Route("api/declarations")]
    public class DeclarationsController(IDeclarationService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<DeclarationListItemDto>>> GetList(
            [FromQuery] DeclarationQuery query, CancellationToken ct)
            => Ok(await service.GetPagedAsync(query, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DeclarationDetailDto>> GetById(Guid id, CancellationToken ct)
            => Ok(await service.GetByIdAsync(id, ct));

        /// <summary>Yeni bəyannamə (Draft) yaradır və rüsumu hesablayır.</summary>
        [HttpPost, Authorize(Policy = Policies.DeclarantOnly)]
        public async Task<ActionResult<DeclarationDetailDto>> Create(SaveDeclarationRequest request, CancellationToken ct)
        {
            var created = await service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Qaralamanı (Draft) yeniləyir.</summary>
        [HttpPut("{id:guid}"), Authorize(Policy = Policies.DeclarantOnly)]
        public async Task<ActionResult<DeclarationDetailDto>> Update(
            Guid id, SaveDeclarationRequest request, CancellationToken ct)
            => Ok(await service.UpdateAsync(id, request, ct));

        /// <summary>Status dəyişikliyi. Keçidin icazəsi rola görə yoxlanır.</summary>
        [HttpPost("{id:guid}/status")]
        public async Task<ActionResult<DeclarationDetailDto>> ChangeStatus(
            Guid id, ChangeStatusRequest request, CancellationToken ct)
            => Ok(await service.ChangeStatusAsync(id, request, ct));
    }

}
