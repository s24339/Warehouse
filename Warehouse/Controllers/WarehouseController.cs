using Microsoft.AspNetCore.Mvc;
using Warehouse.Dto;
using Warehouse.Exceptions;
using Warehouse.Services;
using Warehouse.Services.Warehouse.Services;


namespace Warehouse.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterProductInWarehouseAsync([FromBody] RegisterProductInWarehouseRequestDTO dto)
    {
        try
        {
            var WarehouseProdId = await _warehouseService.RegisterProductInWarehouseAsync(dto);
            return Ok(WarehouseProdId);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}