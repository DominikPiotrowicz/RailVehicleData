using Microsoft.AspNetCore.Mvc;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;

namespace RailVehicleData.WebAPI.Controllers;

/// <summary>
/// REST API controller for vehicle management operations.
/// Provides endpoints for creating, retrieving, updating, and deleting vehicles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all vehicles.
    /// </summary>
    /// <returns>Collection of all vehicles in the system.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAllVehicles()
    {
        try
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all vehicles");
            return StatusCode(500, new { message = "An error occurred while retrieving vehicles." });
        }
    }

    /// <summary>
    /// Get a vehicle by ID.
    /// </summary>
    /// <param name="id">Vehicle ID (GUID)</param>
    /// <returns>Vehicle data if found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetVehicleById(Guid id)
    {
        try
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound(new { message = $"Vehicle with ID {id} not found." });
            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle {VehicleId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the vehicle." });
        }
    }

    /// <summary>
    /// Create a new vehicle.
    /// </summary>
    /// <param name="vehicleDto">Vehicle creation data including traction systems.</param>
    /// <returns>Created vehicle with assigned ID.</returns>
    [HttpPost]
    public async Task<ActionResult<VehicleDto>> CreateVehicle([FromBody] NewVehicleDto vehicleDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdVehicle = await _vehicleService.CreateStandaloneVehicleAsync(vehicleDto);
            return CreatedAtAction(nameof(GetVehicleById), new { id = createdVehicle.VehicleId }, createdVehicle);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid vehicle data provided");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            return StatusCode(500, new { message = "An error occurred while creating the vehicle." });
        }
    }

    /// <summary>
    /// Get vehicles by manufacturer.
    /// </summary>
    /// <param name="manufacturer">Manufacturer name (e.g., "Siemens", "Bombardier")</param>
    /// <returns>Collection of vehicles by specified manufacturer.</returns>
    [HttpGet("by-manufacturer/{manufacturer}")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetByManufacturer(string manufacturer)
    {
        try
        {
            var vehicles = await _vehicleService.GetVehiclesByManufacturerAsync(manufacturer);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by manufacturer {Manufacturer}", manufacturer);
            return StatusCode(500, new { message = "An error occurred while retrieving vehicles." });
        }
    }

    /// <summary>
    /// Get vehicles by model.
    /// </summary>
    /// <param name="model">Model designation (e.g., "EU07", "Class 395")</param>
    /// <returns>Collection of vehicles with specified model.</returns>
    [HttpGet("by-model/{model}")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetByModel(string model)
    {
        try
        {
            var vehicles = await _vehicleService.GetVehiclesByModelAsync(model);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by model {Model}", model);
            return StatusCode(500, new { message = "An error occurred while retrieving vehicles." });
        }
    }

    /// <summary>
    /// Get vehicles by manufactured year.
    /// </summary>
    /// <param name="year">Manufacturing year (e.g., 2020)</param>
    /// <returns>Collection of vehicles manufactured in specified year.</returns>
    [HttpGet("by-year/{year}")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetByYear(int year)
    {
        try
        {
            if (year < 1800 || year > DateTime.Now.Year)
                return BadRequest(new { message = "Invalid year. Must be between 1800 and current year." });

            var vehicles = await _vehicleService.GetVehiclesByManufacturedYearAsync(year);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles by year {Year}", year);
            return StatusCode(500, new { message = "An error occurred while retrieving vehicles." });
        }
    }

    /// <summary>
    /// Get all active (not decommissioned) vehicles.
    /// </summary>
    /// <returns>Collection of currently active vehicles.</returns>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetActiveVehicles()
    {
        try
        {
            var vehicles = await _vehicleService.GetActiveVehiclesAsync();
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active vehicles");
            return StatusCode(500, new { message = "An error occurred while retrieving active vehicles." });
        }
    }

    /// <summary>
    /// Get all standalone vehicles (not part of a multiple unit).
    /// </summary>
    /// <returns>Collection of standalone vehicles.</returns>
    [HttpGet("standalone")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetStandaloneVehicles()
    {
        try
        {
            var vehicles = await _vehicleService.GetStandaloneVehiclesAsync();
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving standalone vehicles");
            return StatusCode(500, new { message = "An error occurred while retrieving standalone vehicles." });
        }
    }

    /// <summary>
    /// Add a traction system to an existing vehicle.
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <param name="tractionSystemDto">Traction system configuration (Electric, Diesel, or Steam)</param>
    /// <returns>Updated vehicle with new traction system.</returns>
    [HttpPost("{vehicleId}/traction-systems")]
    public async Task<ActionResult<VehicleDto>> AddTractionSystem(Guid vehicleId, [FromBody] NewTractionSystemDto tractionSystemDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedVehicle = await _vehicleService.AddTractionSystemAsync(vehicleId, tractionSystemDto);
            return Ok(updatedVehicle);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid traction system data for vehicle {VehicleId}", vehicleId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding traction system to vehicle {VehicleId}", vehicleId);
            return StatusCode(500, new { message = "An error occurred while adding the traction system." });
        }
    }

    /// <summary>
    /// Decommission a traction system on a vehicle.
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <param name="tractionSystemId">Traction system ID to decommission</param>
    /// <param name="removalDate">Date when traction system was removed</param>
    /// <returns>Updated vehicle with decommissioned traction system.</returns>
    [HttpPost("{vehicleId}/traction-systems/{tractionSystemId}/decommission")]
    public async Task<ActionResult<VehicleDto>> DecommissionTractionSystem(Guid vehicleId, Guid tractionSystemId, [FromQuery] DateTime removalDate)
    {
        try
        {
            var updatedVehicle = await _vehicleService.DecommissionTractionSystemAsync(vehicleId, tractionSystemId, removalDate);
            return Ok(updatedVehicle);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to decommission traction system {TractionSystemId} on vehicle {VehicleId}", tractionSystemId, vehicleId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decommissioning traction system {TractionSystemId} on vehicle {VehicleId}", tractionSystemId, vehicleId);
            return StatusCode(500, new { message = "An error occurred while decommissioning the traction system." });
        }
    }

    /// <summary>
    /// Decommission a vehicle.
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <param name="decommissionDate">Date when vehicle was decommissioned</param>
    /// <returns>Updated vehicle with decommissioned status.</returns>
    [HttpPost("{vehicleId}/decommission")]
    public async Task<ActionResult<VehicleDto>> DecommissionVehicle(Guid vehicleId, [FromQuery] DateTime decommissionDate)
    {
        try
        {
            var decommissionedVehicle = await _vehicleService.DecommissionVehicleAsync(vehicleId, decommissionDate);
            return Ok(decommissionedVehicle);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to decommission vehicle {VehicleId}", vehicleId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decommissioning vehicle {VehicleId}", vehicleId);
            return StatusCode(500, new { message = "An error occurred while decommissioning the vehicle." });
        }
    }
}
