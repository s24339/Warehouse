using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Warehouse.Dto;
using Warehouse.Exceptions;
using Warehouse.Services;
using Warehouse.Services.Warehouse.Services;

namespace Warehouse.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SecondWarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public SecondWarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterProductWithProcedureAsync([FromBody] RegisterProductInWarehouseRequestDTO dto)
    {
        try
        {
            var WarehouseProdId = await _warehouseService.RegisterProductInWarehouseUsingProcedureAsync(dto);
            return Ok(new { WarehouseProdId });
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
}