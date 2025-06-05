using CustomerManager.Business;
using CustomerManager.Repository;
using System.Text.Json;

namespace CustomerManager.Api.Middlewares
{
	public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
	{
		private readonly RequestDelegate _next = next;
		private readonly ILogger<ExceptionMiddleware> _logger = logger;

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context); 
			}
			catch (ExceptionHandlerBuisiness ex)
			{
				_logger.LogError(ex, "Errore proveniente dal layer di Buisiness");
				context.Response.StatusCode = ex.StatusCode;
				context.Response.ContentType = "application/json";
				if(ex.InvolvedElement != null)
				{
					context.Response.Headers.Append("Involved-Element", JsonSerializer.Serialize(ex.InvolvedElement));
				}
				var result = JsonSerializer.Serialize(new
				{
					handler = "ExceptionHandlerBuisiness",
					source = ex.Source,
					message =  ex.Message,
					stackTrace = ex.StackTrace

				});

				await context.Response.WriteAsync(result);
			}
			catch (ExceptionHandlerRepository ex)
			{
				_logger.LogError(ex, "Errore proveniente dal layer di Repository");
				context.Response.StatusCode = ex.StatusCode;
				context.Response.ContentType = "application/json";
				if (ex.InvolvedElement != null)
				{
					context.Response.Headers.Append("Involved-Element", JsonSerializer.Serialize(ex.InvolvedElement));
				}
				var result = JsonSerializer.Serialize(new
				{
					handler = "ExceptionHandlerRepository",
					source = ex.Source,
					message = ex.Message,
					stackTrace = ex.StackTrace
				});

				await context.Response.WriteAsync(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Errore generico non gestito");
				context.Response.StatusCode = 500;
				context.Response.ContentType = "application/json";

				var result = JsonSerializer.Serialize(new
				{
					error = "Errore interno del server",
					details = ex.ToString()
				});

				await context.Response.WriteAsync(result);
			}
		}
	}

}
