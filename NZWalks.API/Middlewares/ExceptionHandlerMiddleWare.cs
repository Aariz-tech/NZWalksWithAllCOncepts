using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddleWare
    {
        private readonly ILogger<ExceptionHandlerMiddleWare> logger;
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleWare(ILogger<ExceptionHandlerMiddleWare> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                //Log the exception 
                logger.LogError(ex, $"{errorId} : {ex.Message}");
                //return the custome error in response 
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong! we are looking into resolving this."
                };
                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
