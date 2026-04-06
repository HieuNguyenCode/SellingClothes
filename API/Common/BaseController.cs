using Microsoft.AspNetCore.Mvc;

namespace API.Common;

public class BaseController : ControllerBase
{
    protected IActionResult Result(dynamic serviceResponse)
    {
        int statusCode = serviceResponse.Status;
        return StatusCode(statusCode, serviceResponse);
    }
}
