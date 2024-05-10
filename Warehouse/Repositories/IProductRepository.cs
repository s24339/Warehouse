namespace Warehouse.Repositories
{
    public interface IProductRepository
    {
        Task<bool> ProductExistsAsync(int productId);
    }
}
