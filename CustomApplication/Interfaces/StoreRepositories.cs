using CustomApplication.DTOs;
using CustomDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Interfaces
{
    public interface IStoreRepository
    {
        Task<IReadOnlyList<StoreProductDto>> ListProductsAsync(CancellationToken ct);
        Task<IReadOnlyList<StoreProduct>> GetProductsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
        Task<IReadOnlyList<StoreOrderDto>> GetOrdersAsync(Guid userId, CancellationToken ct);
        void AddOrder(StoreOrder order);
    }

}
