using Clinic.Exceptions;
using System.Net;

namespace Clinic.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger,RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                var (statusCode, message) = ex switch
                {
                    NotFoundException => (HttpStatusCode.NotFound, ex.Message),
                    DuplicateException => (HttpStatusCode.Conflict, ex.Message),
                    BadRequestException => (HttpStatusCode.BadRequest, ex.Message),
                    _ => (HttpStatusCode.InternalServerError, "Something Wrong"),
                };


                if(statusCode == HttpStatusCode.InternalServerError)
                {
                    _logger.LogError("Exception : {message}", ex.Message);
                }
                else
                {
                    _logger.LogInformation("Handeled Exception : {Type} - {message}",ex.GetType().Name,ex.Message);
                }

                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = (int)statusCode,
                    error = message,
                });

            }
        }



    }
}
