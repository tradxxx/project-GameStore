﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using project_GameStore_server.Service;

namespace project_GameStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        LocalAuthService _localAuthService = LocalAuthService.GetInstance();

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

        [HttpPost]
        [Route("signup")]
        public IActionResult SignUp([FromBody] JObject json)
        {
            //Провписать логику регистрации клиента через EntityGateway
        }
    }
}
