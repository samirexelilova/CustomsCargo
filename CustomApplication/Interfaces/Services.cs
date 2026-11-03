using CustomApplication.Common;
using CustomApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct);
        Task LogoutAsync(RefreshRequest request, CancellationToken ct);
        Task<UserDto> CreateStaffAsync(CreateStaffRequest request, CancellationToken ct);
        Task<UserDto> GetCurrentAsync(CancellationToken ct);
    }

    public interface IDutyService
    {
        Task<DutyCalculationResponse> CalculateAsync(CalculateDutyRequest request, CancellationToken ct);
    }

    public interface IDeclarationService
    {
        Task<PagedResult<DeclarationListItemDto>> GetPagedAsync(DeclarationQuery query, CancellationToken ct);
        Task<DeclarationDetailDto> GetByIdAsync(Guid id, CancellationToken ct);
        Task<DeclarationDetailDto> CreateAsync(SaveDeclarationRequest request, CancellationToken ct);
        Task<DeclarationDetailDto> UpdateAsync(Guid id, SaveDeclarationRequest request, CancellationToken ct);
        Task<DeclarationDetailDto> ChangeStatusAsync(Guid id, ChangeStatusRequest request, CancellationToken ct);
    }

    public interface IReferenceDataService
    {
        Task<IReadOnlyList<HsCodeDto>> SearchHsCodesAsync(string? search, CancellationToken ct);
        Task<IReadOnlyList<WarehouseDto>> ListWarehousesAsync(CancellationToken ct);
    }

}
