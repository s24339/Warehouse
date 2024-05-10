using Warehouse.Dto;

namespace Warehouse.Services
{
    namespace Warehouse.Services
    {
        public interface IWarehouseService
        {
            Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto);
            Task<int> RegisterProductInWarehouseUsingProcedureAsync(RegisterProductInWarehouseRequestDTO dto);
        }
    }

}
