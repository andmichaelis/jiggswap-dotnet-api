using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Jiggswap.Api.Controllers
{
    public class BaseJiggswapController : ControllerBase
    {
        public IMediator Mediator { get; }

        public BaseJiggswapController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}