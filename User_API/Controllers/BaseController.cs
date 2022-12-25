using MediatR;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace User_API.Controllers
{
    [Route("userapi/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IMediator? _mediator;
        private ILogger? _loggerInstance;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ILogger Logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger>();
    }
}