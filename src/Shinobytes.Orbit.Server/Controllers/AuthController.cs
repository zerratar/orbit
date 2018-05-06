using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shinobytes.Orbit.Server;

namespace Orbit.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IPlayerSessionManager sessionManager;
        private readonly IPlayerAuthenticator auth;

        public AuthController(IPlayerSessionManager sessionManager, IPlayerAuthenticator auth)
        {
            this.sessionManager = sessionManager;
            this.auth = auth;
        }

        [HttpPost("login")]
        public string Login([FromBody]string username, [FromBody]string password)
        {
            // return status or session id
            var player = auth.Authenticate(username, password);
            if (player != null)
            {
                // check if a session exists with the username
                // if so, terminate it. cannot be logged in on multiple devices.

                if (sessionManager.TryGetByUsername(username, out var existingSession))
                {
                    sessionManager.EndSession(existingSession);
                }

                return sessionManager.BeginSession(HttpContext.Session.Id, player);
            }

            return null;
        }

        [HttpGet("logout")]
        public void Logout()
        {
            if (this.sessionManager.TryGet(this.HttpContext.Session.Id, out var session))
            {
                sessionManager.EndSession(session);
            }
        }
    }
}