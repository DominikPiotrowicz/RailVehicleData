using Microsoft.AspNetCore.Mvc;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.WebAPI.Controllers;

/// <summary>
/// REST API controller for multiple unit (EMU/DMU) management operations.
/// Provides endpoints for train set operations including composition management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MultipleUnitsController : ControllerBase
{
    private readonly IMultipleUnitService _multipleUnitService;
    private readonly ILogger<MultipleUnitsController> _logger;

    public MultipleUnitsController(IMultipleUnitService multipleUnitService, ILogger<MultipleUnitsController> logger)
    {
        _multipleUnitService = multipleUnitService;
        _logger = logger;
    }

    /// <summary>
    /// Get all multiple units.
    /// </summary>
    /// <returns>Collection of all multiple units (EMU/DMU train sets) in the system.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MultipleUnitDto>>> GetAllMultipleUnits()
    {
        try
        {
            var multipleUnits = await _multipleUnitService.GetAllMultipleUnitsAsync();
            return Ok(multipleUnits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all multiple units");
            return StatusCode(500, new { message = "An error occurred while retrieving multiple units." });
        }
    }

    /// <summary>
    /// Get a multiple unit by ID.
    /// </summary>
    /// <param name="id">Multiple unit ID (GUID)</param>
    /// <returns>Multiple unit data if found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<MultipleUnitDto>> GetMultipleUnitById(Guid id)
    {
        try
        {
            var multipleUnit = await _multipleUnitService.GetMultipleUnitByIdAsync(id);
            if (multipleUnit == null)
                return NotFound(new { message = $"Multiple unit with ID {id} not found." });
            return Ok(multipleUnit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multiple unit {MultipleUnitId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the multiple unit." });
        }
    }

    /// <summary>
    /// Create a new multiple unit (train set).
    /// </summary>
    /// <param name="multipleUnitDto">Multiple unit creation data with composition details.</param>
    /// <returns>Created multiple unit with assigned ID.</returns>
    [HttpPost]
    public async Task<ActionResult<MultipleUnitDto>> CreateMultipleUnit([FromBody] NewMultipleUnitDto multipleUnitDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMultipleUnit = await _multipleUnitService.CreateMultipleUnitAsync(multipleUnitDto);
            return CreatedAtAction(nameof(GetMultipleUnitById), new { id = createdMultipleUnit.MultipleUnitId }, createdMultipleUnit);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid multiple unit data provided");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating multiple unit");
            return StatusCode(500, new { message = "An error occurred while creating the multiple unit." });
        }
    }

    /// <summary>
    /// Get a multiple unit by its designation (e.g., "Class 395").
    /// </summary>
    /// <param name="designation">Multiple unit designation/class name</param>
    /// <returns>Multiple unit matching the designation if found.</returns>
    [HttpGet("by-designation/{designation}")]
    public async Task<ActionResult<MultipleUnitDto>> GetByDesignation(string designation)
    {
        try
        {
            var multipleUnit = await _multipleUnitService.GetMultipleUnitByDesignationAsync(designation);
            if (multipleUnit == null)
                return NotFound(new { message = $"Multiple unit with designation '{designation}' not found." });
            return Ok(multipleUnit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multiple unit by designation {Designation}", designation);
            return StatusCode(500, new { message = "An error occurred while retrieving the multiple unit." });
        }
    }

    /// <summary>
    /// Get multiple units by type (Electric, Diesel, or Hybrid).
    /// </summary>
    /// <param name="type">Multiple unit type ("Electric", "Diesel", or "Hybrid")</param>
    /// <returns>Collection of multiple units of specified type.</returns>
    [HttpGet("by-type/{type}")]
    public async Task<ActionResult<IEnumerable<MultipleUnitDto>>> GetByType(string type)
    {
        try
        {
            if (!Enum.TryParse<MultipleUnitType>(type, ignoreCase: true, out var muType))
                return BadRequest(new { message = "Invalid multiple unit type. Must be 'Electric', 'Diesel', or 'Hybrid'." });

            var multipleUnits = await _multipleUnitService.GetMultipleUnitsByTypeAsync(muType);
            return Ok(multipleUnits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multiple units by type {Type}", type);
            return StatusCode(500, new { message = "An error occurred while retrieving multiple units." });
        }
    }

    /// <summary>
    /// Get all active (not decommissioned) multiple units.
    /// </summary>
    /// <returns>Collection of currently active multiple units.</returns>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MultipleUnitDto>>> GetActiveMultipleUnits()
    {
        try
        {
            var multipleUnits = await _multipleUnitService.GetActiveMultipleUnitsAsync();
            return Ok(multipleUnits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active multiple units");
            return StatusCode(500, new { message = "An error occurred while retrieving active multiple units." });
        }
    }

    /// <summary>
    /// Get all decommissioned multiple units.
    /// </summary>
    /// <returns>Collection of decommissioned multiple units.</returns>
    [HttpGet("decommissioned")]
    public async Task<ActionResult<IEnumerable<MultipleUnitDto>>> GetDecommissionedMultipleUnits()
    {
        try
        {
            var multipleUnits = await _multipleUnitService.GetDecommissionedMultipleUnitsAsync();
            return Ok(multipleUnits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving decommissioned multiple units");
            return StatusCode(500, new { message = "An error occurred while retrieving decommissioned multiple units." });
        }
    }

    /// <summary>
    /// Get all incomplete multiple units (cars not yet fully assigned).
    /// </summary>
    /// <returns>Collection of multiple units that need additional cars.</returns>
    [HttpGet("incomplete")]
    public async Task<ActionResult<IEnumerable<MultipleUnitDto>>> GetIncompleteMultipleUnits()
    {
        try
        {
            var multipleUnits = await _multipleUnitService.GetIncompleteMultipleUnitsAsync();
            return Ok(multipleUnits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving incomplete multiple units");
            return StatusCode(500, new { message = "An error occurred while retrieving incomplete multiple units." });
        }
    }

    /// <summary>
    /// Add a car to a multiple unit.
    /// </summary>
    /// <param name="multipleUnitId">Multiple unit ID</param>
    /// <param name="carId">Vehicle ID of the car to add</param>
    /// <returns>Updated multiple unit with the new car.</returns>
    [HttpPost("{multipleUnitId}/cars/{carId}")]
    public async Task<ActionResult<MultipleUnitDto>> AddCar(Guid multipleUnitId, Guid carId)
    {
        try
        {
            var updatedMultipleUnit = await _multipleUnitService.AddCarToMultipleUnitAsync(multipleUnitId, carId);
            return Ok(updatedMultipleUnit);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to add car {CarId} to multiple unit {MultipleUnitId}", carId, multipleUnitId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding car {CarId} to multiple unit {MultipleUnitId}", carId, multipleUnitId);
            return StatusCode(500, new { message = "An error occurred while adding the car." });
        }
    }

    /// <summary>
    /// Remove a car from a multiple unit.
    /// </summary>
    /// <param name="multipleUnitId">Multiple unit ID</param>
    /// <param name="carId">Vehicle ID of the car to remove</param>
    /// <returns>Updated multiple unit with the car removed.</returns>
    [HttpDelete("{multipleUnitId}/cars/{carId}")]
    public async Task<ActionResult<MultipleUnitDto>> RemoveCar(Guid multipleUnitId, Guid carId)
    {
        try
        {
            var updatedMultipleUnit = await _multipleUnitService.RemoveCarFromMultipleUnitAsync(multipleUnitId, carId);
            return Ok(updatedMultipleUnit);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to remove car {CarId} from multiple unit {MultipleUnitId}", carId, multipleUnitId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing car {CarId} from multiple unit {MultipleUnitId}", carId, multipleUnitId);
            return StatusCode(500, new { message = "An error occurred while removing the car." });
        }
    }

    /// <summary>
    /// Decommission a multiple unit.
    /// </summary>
    /// <param name="multipleUnitId">Multiple unit ID</param>
    /// <param name="decommissionDate">Date when multiple unit was decommissioned</param>
    /// <returns>Updated multiple unit with decommissioned status.</returns>
    [HttpPost("{multipleUnitId}/decommission")]
    public async Task<ActionResult<MultipleUnitDto>> DecommissionMultipleUnit(Guid multipleUnitId, [FromQuery] DateTime decommissionDate)
    {
        try
        {
            var decommissionedMultipleUnit = await _multipleUnitService.DecommissionMultipleUnitAsync(multipleUnitId, decommissionDate);
            return Ok(decommissionedMultipleUnit);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to decommission multiple unit {MultipleUnitId}", multipleUnitId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decommissioning multiple unit {MultipleUnitId}", multipleUnitId);
            return StatusCode(500, new { message = "An error occurred while decommissioning the multiple unit." });
        }
    }
}
