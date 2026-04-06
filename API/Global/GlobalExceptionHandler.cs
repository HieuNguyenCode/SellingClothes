using Microsoft.AspNetCore.Diagnostics;
using Services.Dto.Responses;

namespace API.Global;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var requestMethod = httpContext.Request.Method;
        var requestPath = httpContext.Request.Path;
        logger.LogError(
            exception,
            "Lỗi hệ thống tại API [{Method} {Path}]: {Message}",
            requestMethod,
            requestPath,
            exception.Message
        );

        var response = new ServiceResponse
        {
            Status = 500,
            Message = "Đã xảy ra lỗi hệ thống. Vui lòng liên hệ Admin."
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
