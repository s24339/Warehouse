namespace Warehouse.Repositories
{
    public interface IOrderRepository
    {
        Task<int?> OrderExistsAsync(int productId, int amount, DateTime createdAt);
        Task<bool> IsOrderFulfilledAsync(int orderId);
    }
}
