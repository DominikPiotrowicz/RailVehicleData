using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RailVehicleData.Application.Dto;
using RailVehicleData.Application.Interfaces;
using RailVehicleData.Domain.Entities;
using RailVehicleData.WebAPI.Controllers;
using Xunit;

namespace RailVehicleData.Tests.Controllers
{
	/// <summary>
	/// Testy jednostkowe dla RailVehicleController
	/// </summary>
	public class RailVehicleControllerTests
	{
		private readonly Mock<IVehicleService> _mockService;
		private readonly Mock<ILogger<RailVehicleController>> _mockLogger;
		private readonly RailVehicleController _controller;

		public RailVehicleControllerTests()
		{
			_mockService = new Mock<IVehicleService>();
			_mockLogger = new Mock<ILogger<RailVehicleController>>();
			_controller = new RailVehicleController(_mockService.Object, _mockLogger.Object);
		}

		#region GET Tests

		[Fact]
		public async Task Get_ShouldReturnOkWithVehicles_WhenVehiclesExist()
		{
			// Arrange
			var vehicles = new List<VehicleDto>
			{
				new VehicleDto { VehicleId = 1, Model = "SM42", Manufacturer = "Fablok" },
				new VehicleDto { VehicleId = 2, Model = "EU07", Manufacturer = "Pafawag" }
			};

			_mockService.Setup(s => s.GetAllVehicleAsync()).ReturnsAsync(vehicles);

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			okResult!.Value.Should().BeEquivalentTo(vehicles);
			_mockService.Verify(s => s.GetAllVehicleAsync(), Times.Once);
		}

		[Fact]
		public async Task Get_ShouldReturnOkWithEmptyList_WhenNoVehiclesExist()
		{
			// Arrange
			_mockService.Setup(s => s.GetAllVehicleAsync()).ReturnsAsync(new List<VehicleDto>());

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			var vehicles = okResult!.Value as IEnumerable<VehicleDto>;
			vehicles.Should().BeEmpty();
		}

		[Fact]
		public async Task Get_ShouldReturnInternalServerError_WhenExceptionOccurs()
		{
			// Arrange
			_mockService.Setup(s => s.GetAllVehicleAsync()).ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().BeOfType<ObjectResult>();
			var objectResult = result as ObjectResult;
			objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}

		#endregion

		#region GetById Tests

		[Fact]
		public async Task GetById_ShouldReturnOkWithVehicle_WhenVehicleExists()
		{
			// Arrange
			var vehicle = new VehicleDto
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok"
			};

			_mockService.Setup(s => s.GetVehicleByIdAsync(1)).ReturnsAsync(vehicle);

			// Act
			var result = await _controller.GetById(1);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			okResult!.Value.Should().BeEquivalentTo(vehicle);
			_mockService.Verify(s => s.GetVehicleByIdAsync(1), Times.Once);
		}

		[Fact]
		public async Task GetById_ShouldReturnNotFound_WhenVehicleDoesNotExist()
		{
			// Arrange
			_mockService.Setup(s => s.GetVehicleByIdAsync(999)).ReturnsAsync((VehicleDto?)null);

			// Act
			var result = await _controller.GetById(999);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			var notFoundResult = result as NotFoundObjectResult;
			notFoundResult!.Value.Should().Be("Pojazd o ID 999 nie został znaleziony");
		}

		[Fact]
		public async Task GetById_ShouldReturnBadRequest_WhenIdIsZero()
		{
			// Act
			var result = await _controller.GetById(0);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult!.Value.Should().Be("ID pojazdu musi być większe od 0");
		}

		[Fact]
		public async Task GetById_ShouldReturnBadRequest_WhenIdIsNegative()
		{
			// Act
			var result = await _controller.GetById(-1);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task GetById_ShouldReturnInternalServerError_WhenExceptionOccurs()
		{
			// Arrange
			_mockService.Setup(s => s.GetVehicleByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.GetById(1);

			// Assert
			result.Should().BeOfType<ObjectResult>();
			var objectResult = result as ObjectResult;
			objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}

		#endregion

		#region Create Tests

		[Fact]
		public async Task Create_ShouldReturnCreated_WhenValidVehicleProvided()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto
			{
				Model = "SM42",
				Manufacturer = "Fablok",
				ProductionYear = 1975
			};

			var createdVehicle = new Vehicle
			{
				VehicleId = 1,
				Model = "SM42",
				Manufacturer = "Fablok"
			};

			_mockService.Setup(s => s.AddNew<Vehicle>(newVehicleDto)).Returns(createdVehicle);

			// Act
			var result = await _controller.Create(newVehicleDto);

			// Assert
			result.Should().BeOfType<CreatedAtActionResult>();
			var createdResult = result as CreatedAtActionResult;
			createdResult!.ActionName.Should().Be(nameof(RailVehicleController.GetById));
			createdResult.RouteValues!["id"].Should().Be(1);
			createdResult.Value.Should().Be(createdVehicle);
		}

		[Fact]
		public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto();
			_controller.ModelState.AddModelError("Model", "Model jest wymagany");

			// Act
			var result = await _controller.Create(newVehicleDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult!.Value.Should().BeOfType<SerializableError>();
		}

		[Fact]
		public async Task Create_ShouldReturnInternalServerError_WhenExceptionOccurs()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto { Model = "SM42" };
			_mockService.Setup(s => s.AddNew<Vehicle>(newVehicleDto)).Throws(new Exception("Database error"));

			// Act
			var result = await _controller.Create(newVehicleDto);

			// Assert
			result.Should().BeOfType<ObjectResult>();
			var objectResult = result as ObjectResult;
			objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}

		#endregion

		#region CreateElectricVehicle Tests

		[Fact]
		public async Task CreateElectricVehicle_ShouldReturnCreated_WhenValidVehicleProvided()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto
			{
				Model = "EU07",
				Manufacturer = "Pafawag",
				Voltage = "3000V",
				CurrentType = "DC"
			};

			var createdVehicle = new ElectricVehicle
			{
				VehicleId = 1,
				Model = "EU07",
				Voltage = "3000V"
			};

			_mockService.Setup(s => s.AddNew<ElectricVehicle>(newVehicleDto)).Returns(createdVehicle);

			// Act
			var result = await _controller.CreateElectricVehicle(newVehicleDto);

			// Assert
			result.Should().BeOfType<CreatedAtActionResult>();
			var createdResult = result as CreatedAtActionResult;
			createdResult!.Value.Should().Be(createdVehicle);
			createdResult.RouteValues!["id"].Should().Be(1);
		}

		[Fact]
		public async Task CreateElectricVehicle_ShouldReturnBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var newVehicleDto = new NewVehicleDto();
			_controller.ModelState.AddModelError("Model", "Model jest wymagany");

			// Act
			var result = await _controller.CreateElectricVehicle(newVehicleDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		#endregion

		#region Update Tests

		[Fact]
		public async Task Update_ShouldReturnNoContent_WhenUpdateSucceeds()
		{
			// Arrange
			var updateDto = new NewVehicleDto { Model = "SM42-Updated" };
			_mockService.Setup(s => s.UpdateVehicleAsync(1, updateDto)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Update(1, updateDto);

			// Assert
			result.Should().BeOfType<NoContentResult>();
			_mockService.Verify(s => s.UpdateVehicleAsync(1, updateDto), Times.Once);
		}

		[Fact]
		public async Task Update_ShouldReturnBadRequest_WhenIdIsInvalid()
		{
			// Arrange
			var updateDto = new NewVehicleDto { Model = "Test" };

			// Act
			var result = await _controller.Update(0, updateDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var updateDto = new NewVehicleDto();
			_controller.ModelState.AddModelError("Model", "Model jest wymagany");

			// Act
			var result = await _controller.Update(1, updateDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Update_ShouldReturnNotFound_WhenVehicleDoesNotExist()
		{
			// Arrange
			var updateDto = new NewVehicleDto { Model = "Test" };
			_mockService.Setup(s => s.UpdateVehicleAsync(999, updateDto))
				.ThrowsAsync(new KeyNotFoundException("Pojazd o ID 999 nie został znaleziony"));

			// Act
			var result = await _controller.Update(999, updateDto);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		#endregion

		#region Delete Tests

		[Fact]
		public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
		{
			// Arrange
			_mockService.Setup(s => s.DeleteVehicleAsync(1)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Delete(1);

			// Assert
			result.Should().BeOfType<NoContentResult>();
			_mockService.Verify(s => s.DeleteVehicleAsync(1), Times.Once);
		}

		[Fact]
		public async Task Delete_ShouldReturnBadRequest_WhenIdIsInvalid()
		{
			// Act
			var result = await _controller.Delete(0);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Delete_ShouldReturnNotFound_WhenVehicleDoesNotExist()
		{
			// Arrange
			_mockService.Setup(s => s.DeleteVehicleAsync(999))
				.ThrowsAsync(new KeyNotFoundException("Pojazd o ID 999 nie został znaleziony"));

			// Act
			var result = await _controller.Delete(999);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task Delete_ShouldReturnInternalServerError_WhenExceptionOccurs()
		{
			// Arrange
			_mockService.Setup(s => s.DeleteVehicleAsync(1)).ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.Delete(1);

			// Assert
			result.Should().BeOfType<ObjectResult>();
			var objectResult = result as ObjectResult;
			objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}

		#endregion

		#region Constructor Tests

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
		{
			// Act
			Action act = () => new RailVehicleController(null!, _mockLogger.Object);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParameterName("vehicleService");
		}

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
		{
			// Act
			Action act = () => new RailVehicleController(_mockService.Object, null!);

			// Assert
			act.Should().Throw<ArgumentNullException>()
				.WithParameterName("logger");
		}

		#endregion
	}
}
