using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using project_GameStore_dblayer;
using project_GameStore_models;
using project_GameStore_models.Models;
using project_GameStore_server.Service;

namespace project_GameStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        LocalAuthService _localAuthService = LocalAuthService.GetInstance();
        readonly EntityGateway _db = new();

        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ? Request.Headers["Token"] : Guid.Empty.ToString());
        /// <summary>
        /// Auth
        /// </summary>
        /// <returns>message with token</returns>

        [HttpPost]
        public IActionResult AuthPost(string username,string password)
        {
            try
            {
                var token = _localAuthService.Auth(username, password);
                return Ok(new
                {
                    status = "ok",
                    token
                });
            }
            catch (Exception e)
            {

                return Unauthorized(new
                {
                    status = "fail",
                    message = e.Message
                });
            }
        }

        /// <summary>
        /// Get rights of authentified user
        /// </summary>
        /// <returns>rights as string</returns>
        [HttpPost]
        [Route("rights")]
        public IActionResult CheckRights()
        {
            try
            {
                return Ok(new
                {
                    status = "ok",
                    rights = _localAuthService.GetRole(Token)
                });
            }
            catch (Exception E)
            {

                return Unauthorized(new
                {
                    status = "fail",
                    message=E.Message
                });
            }
        }

        /// <summary>
        /// Get user information Json
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("user")]
        public IActionResult GetUserInfo()
        {
            try
            {
                return Ok(new
                {
                    status = "ok",
                    user = _localAuthService.GetClient(Token)                    
                });
            }
            catch (Exception E)
            {

                return Unauthorized(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }



        /// <summary>
        /// register new client
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("signup")]
        public IActionResult SignUp([FromBody] JObject json)
        {
            //Прописать логику регистрации клиента через EntityGateway
            try
            {
                if (_db.GetClients(x => x.Login == json["login"]?.ToString()).Any())
                    throw new Exception("User with this login exists");
                Client potentialClient = new()
                {
                    Login = json["login"]?.ToString() ?? throw new Exception("Login is missing"),
                    Password = Extentions.ComputeSha256Hash(json["password"]?.ToString() ?? throw new Exception("Password is missing")),
                    Name = json["name"]?.ToString() ?? throw new Exception("Name is missing"),
                    Role = Role.User
                };
                 _db.AddOrUpdate(potentialClient);
                return Ok(new
                {
                    status = "ok"
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
