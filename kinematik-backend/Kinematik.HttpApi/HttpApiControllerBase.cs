using Microsoft.AspNetCore.Mvc;

namespace Kinematik.HttpApi
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class HttpApiControllerBase : ControllerBase
    {
    }
}