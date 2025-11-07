using Microsoft.AspNetCore.Mvc;
using RailVehicleData.Aplication.Dto;
using RailVehicleData.Aplication.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.Infrastrcture.Repositories;

namespace RailVehicleData.WebAPI.Controllers
{
	[Route("api/[controller]")]
	public class RailVehicleController : ControllerBase
	{
		private readonly IVehicleService _vehicleService;

		public RailVehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var vehicles = await _vehicleService.GetAllVehicleAsync();
			return Ok(vehicles);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById([FromRoute] int id)
		{
			var vehicleDtos = await _vehicleService.GetVehicleByIdAsync(id);
			return Ok(vehicleDtos);
		}

		[HttpPost]
		public IActionResult Create(NewVehicleDto newVehicle)
		{
			var vehicle = _vehicleService.AddNew<Vehicle>(newVehicle);
			return Created($"api/Railvehicle/{vehicle.VehicleId}", vehicle);
		}


		[HttpPost("ElectricVehicles")]
		public IActionResult CreateElectricVehicle(NewVehicleDto newVehicle)
		{
			var vehicle = _vehicleService.AddNew<ElectricVehicle>(newVehicle);
			return Created($"api/Railvehicle/{vehicle.VehicleId}", vehicle);
		}
	}
}
