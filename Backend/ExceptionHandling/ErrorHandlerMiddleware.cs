using Microsoft.AspNetCore.Mvc;
namespace Backend.ExceptionHandling;
public class ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger) : IMiddleware {
    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        try {await next(context);}
        catch (Exception error) {
            if (context.Response.HasStarted) throw;
            var status = error switch {BadRequestException => 400, KeyNotFoundException => 404, UnauthorizedAccessException => 401, _ => 500};
            if (status == 500) logger.LogError(error, "Unhandled request error");
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails {Status = status, Title = "Request failed", Detail = status == 500 ? "An unexpected error occurred." : error.Message});
        }
    }
}
