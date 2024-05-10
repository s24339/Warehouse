namespace Warehouse.Repositories
{
    public interface IWarehouseRepository
    {
        public Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder, int amount,DateTime createdAt);
        Task<bool> WarehouseExistsAsync(int warehouseId);

        public Task<int> RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, int amount, DateTime createdAt);
    }
}