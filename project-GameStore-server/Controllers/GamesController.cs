using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_GameStore_dblayer;
using project_GameStore_models;
using project_GameStore_models.Models;
using project_GameStore_server.Service;

namespace project_GameStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ? Request.Headers["Token"] : Guid.Empty.ToString());

        /// <summary>
        /// Get Game news
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/news")]
        public IActionResult GetNewsGame()
        {
            return Ok();
        }

        /// <summary>
        /// Get all games
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new
            {
                status = "ok",
                games = _db.GetGames()
            });
        }

        /// <summary>
        /// Get game by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(Guid id)
        {
            var potentialGame = _db.GetGames(x => x.Id == id).FirstOrDefault();
            if (potentialGame is not null)
                return Ok(new
                {
                    status = "ok",
                    game = potentialGame
                });
            else
                return NotFound(new
                {
                    status = "fail",
                    message = $"There is no games with this id {id}! "
                });
        }

        [HttpPost]
        public IActionResult PostGame([FromBody] Game game)
        {
            if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                return Unauthorized(new
                {
                    status = "fail",
                    message = "You have no rights for that action."
                });

            _db.AddOrUpdate(game);
            return Ok(new
            {
                status = "ok",
                id = game.Id
            });
        }

        
    }
}
