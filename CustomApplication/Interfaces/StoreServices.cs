using CustomApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Interfaces
{
    public interface IStoreService
    {
        Task<IReadOnlyList<StoreProductDto>> ListProductsAsync(CancellationToken ct);
        Task<IReadOnlyList<CargoCompanyDto>> ListCargoCompaniesAsync(CancellationToken ct);
        Task<StoreOrderDto> CreateOrderAsync(CreateStoreOrderRequest request, CancellationToken ct);
        Task<IReadOnlyList<StoreOrderDto>> ListMyOrdersAsync(CancellationToken ct);
    }


}
