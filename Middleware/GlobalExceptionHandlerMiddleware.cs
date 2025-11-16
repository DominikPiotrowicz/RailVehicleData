using System.Net;
using System.Text.Json;

namespace RailVehicleData.WebAPI.Middleware
{
	/// <summary>
	/// Middleware do globalnej obsługi wyjątków w aplikacji
	/// </summary>
	public class GlobalExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

		public GlobalExceptionHandlerMiddleware(
			RequestDelegate next,
			ILogger<GlobalExceptionHandlerMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Nieobsłużony wyjątek: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

			var response = new ErrorResponse
			{
				Message = "Wystąpił wewnętrzny błąd serwera",
				Details = exception.Message
			};

			switch (exception)
			{
				case KeyNotFoundException:
					context.Response.StatusCode = (int)HttpStatusCode.NotFound;
					response.Message = "Zasób nie został znaleziony";
					break;

				case ArgumentException:
				case ArgumentNullException:
					context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
					response.Message = "Nieprawidłowe dane wejściowe";
					break;

				case UnauthorizedAccessException:
					context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					response.Message = "Brak autoryzacji";
					break;

				default:
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					response.Details = "Skontaktuj się z administratorem systemu";
					break;
			}

			var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			return context.Response.WriteAsync(jsonResponse);
		}
	}

	/// <summary>
	/// Model odpowiedzi błędu
	/// </summary>
	public class ErrorResponse
	{
		public string Message { get; set; } = string.Empty;
		public string? Details { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}

	/// <summary>
	/// Metoda rozszerzająca do łatwej rejestracji middleware
	/// </summary>
	public static class GlobalExceptionHandlerMiddlewareExtensions
	{
		public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
		}
	}
}
