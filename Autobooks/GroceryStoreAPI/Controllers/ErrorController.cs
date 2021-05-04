using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
public class ErrorController : ControllerBase
{
    private ILogger<ErrorController> logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    [Route("/error")]
    public ActionResult<Exception> Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context.Error;
        
        logger.LogError(exception, exception.Message);

        return Problem(exception.ToString());
    }
}