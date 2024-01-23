using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ExceptionHandling;
    
public class ErrorHandlerMiddleware : IMiddleware
{
    public ErrorHandlerMiddleware()
    {
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            context.Response.ContentType = "application/json";
            ProblemDetails problemDetails = new ProblemDetails
            {
                Title = "Error",
                Type = "Error",
                Detail = error.Message
            };

            switch(error)
            {
                case BadRequestException e:
                    // custom application error
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    // not found error
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            string json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}