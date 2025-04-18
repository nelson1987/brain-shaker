namespace Brainshaker.Api.MIddlewares;

public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            Console.WriteLine("Error: " + error.Message);
            Console.WriteLine("Error Details: " + context.Request.Path);

            if (error.InnerException != null)
                Console.WriteLine("Inner Error: " + error.InnerException.Message);

            //Return StatusCode, Message, Details as result
            await response.WriteAsync("Error");
        }
    }
}