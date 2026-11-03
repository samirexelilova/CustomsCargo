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
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        void Add(User user);
        void AddRefreshToken(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct);
        Task RevokeAllRefreshTokensAsync(Guid userId, DateTimeOffset now, CancellationToken ct);
    }

    public interface IDeclarationRepository
    {
        Task<PagedResult<DeclarationListItemDto>> GetPagedAsync(DeclarationQuery query, DeclarationScope scope, CancellationToken ct);
        Task<DeclarationDetailDto?> GetDetailAsync(Guid id, DeclarationScope scope, CancellationToken ct);

        Task<Declaration?> GetForUpdateAsync(Guid id, CancellationToken ct);
        void Add(Declaration declaration);
        Task<int> GetNextNumberAsync(CancellationToken ct);
    }

    public interface IHsCodeRepository
    {
        Task<IReadOnlyList<HsCode>> GetByCodesAsync(IReadOnlyCollection<string> codes, CancellationToken ct);
        Task<IReadOnlyList<HsCodeDto>> SearchAsync(string? search, CancellationToken ct);
    }

    public interface IWarehouseRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<WarehouseDto>> ListAsync(CancellationToken ct);
    }

}
