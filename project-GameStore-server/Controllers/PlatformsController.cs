using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_GameStore_dblayer;
using project_GameStore_models.Models;
using project_GameStore_models;
using project_GameStore_server.Service;

namespace project_GameStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ? Request.Headers["Token"]! : Guid.Empty.ToString());


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                games = _db.GetPlatforms()
            });
        }

        [HttpGet]
        [Route("{id}/games")]
        public IActionResult GetGamesInPlatforms([FromRoute] Guid id)
        {
            var potentialPlatform = _db.GetPlatforms(x => x.Id == id).FirstOrDefault();
            return potentialPlatform is null ?
                   NotFound(new
                   {
                       status = "fail",
                       message = $"There is no platform with this id {id}!"
                   }) :
                   Ok(new
                   {
                       status = "ok",
                       games = potentialPlatform.Games
                   });
        }

        [HttpPost]
        public IActionResult Post([FromBody] Platform value)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.User)
                    return Unauthorized(new
                    {
                        status = "fail",
                        message = "You have no rights for this op."
                    });
                _db.AddOrUpdate(value);
                return Ok(new
                {
                    status = "ok",
                    id = value.Id
                });
            }
            catch (Exception E)
            {
                return BadRequest(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }
    }
}
