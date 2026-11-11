using CustomApplication.Common;
using CustomApplication.DTOs;
using CustomDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Interfaces
{
    public interface ICargoParcelRepository
    {
        Task<PagedResult<CargoParcelListItemDto>> GetPagedAsync(CargoParcelQuery query, Guid? ownerId, CancellationToken ct);
        Task<CargoParcelDetailDto?> GetDetailAsync(Guid id, Guid? ownerId, CancellationToken ct);
        Task<CargoParcel?> GetForDeclareAsync(Guid id, CancellationToken ct);
    }

    public interface ICargoCompanyRepository
    {
        Task<IReadOnlyList<CargoCompanyDto>> ListActiveAsync(CancellationToken ct);
    }

    public interface ICargoService
    {
        Task<PagedResult<CargoParcelListItemDto>> GetPagedAsync(CargoParcelQuery query, CancellationToken ct);
        Task<CargoParcelDetailDto> GetByIdAsync(Guid id, CancellationToken ct);
        Task<DeclarationDetailDto> DeclareAsync(Guid id, CancellationToken ct);
    }
}
