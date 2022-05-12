using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik.HttpApi
{
    public class UserController : HttpApiControllerBase
    {

        [HttpGet]
        [SwaggerOperation(
            Summary = "Дістає користувачів"
        )]
        public ActionResult GetUsers()
        {
            return Ok(new string[]
            {
                "John Doe",
                "Sarah Tesla"
            });
        }
    }
}