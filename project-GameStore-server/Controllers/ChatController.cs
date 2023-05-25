using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_GameStore_dblayer;
using project_GameStore_server.Service;

namespace project_GameStore_server.Controllers
{
    [Route("api/projects/{project_id}/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        LocalAuthService _localAuthService = LocalAuthService.GetInstance();
        readonly EntityGateway _db = new();

        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ? Request.Headers["Token"] : Guid.Empty.ToString());

        [HttpGet]
        public async Task<IActionResult> ConnectUser([FromBody] Guid game_id, Guid token)
        {
            var potentialGame = _db.GetGames(x => x.Id == game_id);
            if (potentialGame is null)
                return NotFound(new
                {
                    status = "fail",
                    message = "There is no game with this Id"
                });
            if (!ControllerContext.HttpContext.WebSockets.IsWebSocketRequest)          
                return BadRequest(new
                {
                    status = "fail",
                    message = "Unsupported action!"
                });
            var soket = await ControllerContext.HttpContext.WebSockets.AcceptWebSocketAsync();
            try
            {
                var user = _localAuthService.GetClient(token);
                return Ok();
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
