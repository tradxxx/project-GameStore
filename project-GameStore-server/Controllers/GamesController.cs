using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_GameStore_dblayer;
using project_GameStore_models;
using project_GameStore_models.Models;
using project_GameStore_server.Service;
using static project_GameStore_dblayer.EntityGateway;

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
        /// 

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
                    game = potentialGame,
                    entities = potentialGame.Keys_Game.Select(x => x.Id),
                    category = potentialGame.Category
                });
            else
                return NotFound(new
                {
                    status = "fail",
                    message = $"There is no games with this id {id}! "
                });
        }

        [HttpGet]
        [Route("{id}/subdata")]
        public IActionResult GetSubdataFromGame([FromRoute] Guid id, [FromRoute] GameSubdata subdata)
        {
            var potentialGame = _db.GetGames(x => x.Id == id).FirstOrDefault();
            object res = subdata switch
            {
                GameSubdata.Entities => new
                {
                    status = "ok",
                    employees = potentialGame?.Keys_Game
                },
                GameSubdata.UsedCategory => new
                {
                    status = "ok",
                    usedcategory = potentialGame?.Category
                },
                _ => throw new Exception($"{subdata} instance is not covered.")
            };
            return potentialGame is null
                ? NotFound(new
                {
                    status = "fail",
                    message = $"There is no game with this id {id}!"
                })
                : Ok(res);

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
        /// <summary>
        ///  change subdataIds from game
        /// </summary>
        /// <param name="action"></param>
        /// <param name="id"></param>
        /// <param name="subdataIds">Json array of subdataIds id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/entities/add")]
        [Route("{id}/{subdata}/{action}")]
        public IActionResult ManipulateSubDataInProject([FromRoute] ActionType action,
                                                        [FromRoute] GameSubdata subdata,
                                                        [FromRoute] Guid id,
                                                        [FromBody] Guid[] subdataIds)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                    return Unauthorized(new
                    {
                        status = "fail",
                        message = "You have no rights for this op."
                    });

                var changed = subdata switch
                {
                    GameSubdata.Entities => _db.EntitiesInGame(action, id, subdataIds),
                    GameSubdata.UsedCategory => _db.GamesInCategory(action, id, subdataIds),
                    _ => throw new Exception($"{subdata} instance is not covered.")
                };
                return Ok(new
                {
                    status = "ok",
                    changed
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


#pragma warning disable CS1591
        public enum GameSubdata
        {
            UsedCategory,
            Entities
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
