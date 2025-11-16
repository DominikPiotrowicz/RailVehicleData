using Microsoft.AspNetCore.Mvc;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Domain.Entities;

namespace RailVehicleData.WebAPI.Controllers
{
	/// <summary>
	/// Kontroler API do zarządzania pojazdami szynowymi
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class RailVehicleController : ControllerBase
	{
		private readonly IVehicleService _vehicleService;
		private readonly ILogger<RailVehicleController> _logger;

		public RailVehicleController(
			IVehicleService vehicleService,
			ILogger<RailVehicleController> logger)
		{
			_vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Pobiera wszystkie pojazdy szynowe
		/// </summary>
		/// <returns>Lista wszystkich pojazdów</returns>
		/// <response code="200">Zwraca listę pojazdów</response>
		/// <response code="500">Błąd serwera</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Get()
		{
			try
			{
				_logger.LogInformation("Pobieranie wszystkich pojazdów");
				var vehicles = await _vehicleService.GetAllVehicleAsync();
				return Ok(vehicles);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Błąd podczas pobierania pojazdów");
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Wystąpił błąd podczas pobierania danych");
			}
		}

		/// <summary>
		/// Pobiera pojazd o określonym identyfikatorze
		/// </summary>
		/// <param name="id">Identyfikator pojazdu</param>
		/// <returns>Szczegóły pojazdu</returns>
		/// <response code="200">Zwraca szczegóły pojazdu</response>
		/// <response code="404">Pojazd nie został znaleziony</response>
		/// <response code="400">Nieprawidłowe ID</response>
		/// <response code="500">Błąd serwera</response>
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetById([FromRoute] int id)
		{
			try
			{
				if (id <= 0)
				{
					_logger.LogWarning("Próba pobrania pojazdu z nieprawidłowym ID: {Id}", id);
					return BadRequest("ID pojazdu musi być większe od 0");
				}

				_logger.LogInformation("Pobieranie pojazdu o ID: {Id}", id);
				var vehicleDto = await _vehicleService.GetVehicleByIdAsync(id);

				if (vehicleDto == null)
				{
					_logger.LogWarning("Pojazd o ID {Id} nie został znaleziony", id);
					return NotFound($"Pojazd o ID {id} nie został znaleziony");
				}

				return Ok(vehicleDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Błąd podczas pobierania pojazdu o ID: {Id}", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Wystąpił błąd podczas pobierania danych");
			}
		}

		/// <summary>
		/// Tworzy nowy pojazd standardowy
		/// </summary>
		/// <param name="newVehicle">Dane nowego pojazdu</param>
		/// <returns>Utworzony pojazd</returns>
		/// <response code="201">Pojazd został utworzony</response>
		/// <response code="400">Nieprawidłowe dane wejściowe</response>
		/// <response code="500">Błąd serwera</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create([FromBody] NewVehicleDto newVehicle)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					_logger.LogWarning("Nieprawidłowe dane wejściowe podczas tworzenia pojazdu");
					return BadRequest(ModelState);
				}

				_logger.LogInformation("Tworzenie nowego pojazdu: {Model}", newVehicle.Model);
				var vehicle = _vehicleService.AddNew<Vehicle>(newVehicle);

				return CreatedAtAction(
					nameof(GetById),
					new { id = vehicle.VehicleId },
					vehicle);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Błąd podczas tworzenia pojazdu");
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Wystąpił błąd podczas tworzenia pojazdu");
			}
		}

		/// <summary>
		/// Tworzy nowy pojazd elektryczny
		/// </summary>
		/// <param name="newVehicle">Dane nowego pojazdu elektrycznego</param>
		/// <returns>Utworzony pojazd elektryczny</returns>
		/// <response code="201">Pojazd elektryczny został utworzony</response>
		/// <response code="400">Nieprawidłowe dane wejściowe</response>
		/// <response code="500">Błąd serwera</response>
		[HttpPost("ElectricVehicles")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateElectricVehicle([FromBody] NewVehicleDto newVehicle)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					_logger.LogWarning("Nieprawidłowe dane wejściowe podczas tworzenia pojazdu elektrycznego");
					return BadRequest(ModelState);
				}

				_logger.LogInformation("Tworzenie nowego pojazdu elektrycznego: {Model}", newVehicle.Model);
				var vehicle = _vehicleService.AddNew<ElectricVehicle>(newVehicle);

				return CreatedAtAction(
					nameof(GetById),
					new { id = vehicle.VehicleId },
					vehicle);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Błąd podczas tworzenia pojazdu elektrycznego");
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Wystąpił błąd podczas tworzenia pojazdu elektrycznego");
			}
		}

		/// <summary>
		/// Aktualizuje istniejący pojazd
		/// </summary>
		/// <param name="id">Identyfikator pojazdu</param>
		/// <param name="vehicleDto">Zaktualizowane dane pojazdu</param>
		/// <returns>Status operacji</returns>
		/// <response code="204">Pojazd został zaktualizowany</response>
		/// <response code="404">Pojazd nie został znaleziony</response>
		/// <response code="400">Nieprawidłowe dane wejściowe</response>
		/// <response code="500">Błąd serwera</response>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Update([FromRoute] int id, [FromBody] NewVehicleDto vehicleDto)
		{
			try
			{
				if (id <= 0)
				{
					_logger.LogWarning("Próba aktualizacji pojazdu z nieprawidłowym ID: {Id}", id);
					return BadRequest("ID pojazdu musi być większe od 0");
				}

				if (!ModelState.IsValid)
				{
					_logger.LogWarning("Nieprawidłowe dane wejściowe podczas aktualizacji pojazdu");
					return BadRequest(ModelState);
				}

				_logger.LogInformation("Aktualizacja pojazdu o ID: {Id}", id);
				await _vehicleService.UpdateVehicleAsync(id, vehicleDto);

				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Pojazd o ID {Id} nie został znaleziony", id);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Błąd podczas aktualizacji pojazdu o ID: {Id}", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Wystąpił błąd podczas aktualizacji pojazdu");
			}
		}

		/// <summary>
		/// Usuwa pojazd o określonym identyfikatorze
		/// </summary>
		/// <param name="id">Identyfikator pojazdu do usunięcia</param>
		/// <returns>Status operacji</returns>
		/// <response code="204">Pojazd został usunięty</response>
		/// <response code="404">Pojazd nie został znaleziony</response>
		/// <response code="400">Nieprawidłowe ID</response>
		/// <response code="500">Błąd serwera</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			try
			{
				if (id <= 0)
				{
					_logger.LogWarning("Próba usunięcia pojazdu z nieprawidłowym ID: {Id}", id);
					return BadRequest("ID pojazdu musi być większe od 0");
				}

				_logger.LogInformation("Usuwanie pojazdu o ID: {Id}", id);
				await _vehicleService.DeleteVehicleAsync(id);

				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Pojazd o ID {Id} nie został znaleziony", id);
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Błąd podczas usuwania pojazdu o ID: {Id}", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					"Wystąpił błąd podczas usuwania pojazdu");
			}
		}
	}
}
