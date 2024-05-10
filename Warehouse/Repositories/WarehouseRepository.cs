using System.Data;
using System.Data.SqlClient;
using Warehouse.Exceptions;

namespace Warehouse.Repositories;
public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> WarehouseExistsAsync(int warehouseId)
    {
        try
        {

            await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdWarehouse", warehouseId);

            var doesExist = (int)await command.ExecuteScalarAsync() > 0;
            return doesExist;
        }
        catch
        {
            throw new ConflictException("[ERROR]: Something went wrong while checking product.");
        }
    }
    public async Task<int?> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int idOrder, int amount, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            var priceQuery = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            await using var priceCommand = new SqlCommand(priceQuery, connection);
            priceCommand.Parameters.AddWithValue("@IdProduct", idProduct);
            var pricePerUnit = (decimal)await priceCommand.ExecuteScalarAsync();

            var totalPrice = pricePerUnit * amount;

            var query = "UPDATE \"Order\" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
            await using var command = new SqlCommand(query, connection);
            command.Transaction = (SqlTransaction)transaction;
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();
            
            command.CommandText = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, CreatedAt, Amount, Price) OUTPUT Inserted.IdProductWarehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @CreatedAt, @Amount, @Price);";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Price", totalPrice);
            var idProductWarehouse = (int)await command.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return idProductWarehouse;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<int> RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, int amount, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        try
        {
            await using var command = new SqlCommand("AddProductToWarehouse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);

            var idProductWarehouse = (int)await command.ExecuteScalarAsync();
            return idProductWarehouse;
        }
        catch
        {
            throw new ConflictException("[ERROR]: Something went wrong while checking product.");
        }
    }

}