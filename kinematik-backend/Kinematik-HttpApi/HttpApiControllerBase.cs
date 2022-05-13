using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kinematik_HttpApi
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class HttpApiControllerBase : ControllerBase
    {
    }
}