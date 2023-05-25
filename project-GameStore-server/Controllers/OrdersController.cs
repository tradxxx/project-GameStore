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
    public class OrdersController : ControllerBase
    {
        private readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ? Request.Headers["Token"]! : Guid.Empty.ToString());


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                games = _db.GetOrders()
            });
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id) 
        {
            var potentialOrder = _db.GetOrders(x => x.Id == id).FirstOrDefault();
            return potentialOrder is null
                ? NotFound(new
                {
                    status = "fail",
                    message = $"There is no order with this id {id}!"
                })
                : Ok(new
                {
                    status = "ok",
                    order = potentialOrder,
                    game = potentialOrder.Games.Select(x => x.Id)
                });
        }

        [HttpPost]
        public IActionResult Post([FromBody] Order value)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
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
