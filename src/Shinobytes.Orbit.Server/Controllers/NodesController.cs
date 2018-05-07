using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Shinobytes.Orbit.Server;
using Shinobytes.Orbit.Server.Models;

namespace Orbit.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NodesController : Controller
    {
        private readonly IUserSessionManager sessionManager;
        private readonly INodeRepository nodeRepository;

        private const string NotLoggedIn = "You should login first! :-) POST /api/auth/login";
        public NodesController(IUserSessionManager sessionManager, INodeRepository nodeRepository)
        {
            this.sessionManager = sessionManager;
            this.nodeRepository = nodeRepository;
        }

        [HttpGet]
        public string Get()
        {
            if (!sessionManager.TryGet(HttpContext.Session.Id, out _))
            {
                return NotLoggedIn;
            }

            return "ready";
        }

        [HttpGet("area/{latitude}/{longitude}")]
        public IEnumerable<Node> Around(double latitude, double longitude)
        {
            return this.nodeRepository.GetWithin(latitude, longitude, 2000);
        }

        [HttpPost("hack/{nodeid}")]
        public string HackNode(string nodeid)
        {
            //1. if someone is hacking this node already, you cannot do anything about it.

            //2. if user is already hacking a node, the previous hack should be cleared and node resetted, resources will be lost             

            //3. if this is the first request you do on this node, this will be a "hack initiate". a bit of resources will be placed in the node
            //   each time you try and hack this node. everyone contribute to this pool when trying to hack this node.

            //4. second request, if the state is "hack initiate" a hack-attack will be done with a 'damage' meter

            //5. if node-"health" reaches 0, the node ownership is cleared and player receive resources and exp

            //6. a 1min exclusive timeout is given for the user to takeover this node

            //7. at any time during the "hack-attack" theres a risk of failing. 
            //   Failing will reset the node health and lock you out from hacking it for N seconds, then N minutes on repeated failure.

            //8. on takeover, you exhaust resources that is transfered INTO the node, which will then be the resources the next person gain when hacking

            //the more resources in the node, the harder it is to take over. Owner of node can at anytime extract all resources but will make it easier for others to hack the node.

            if (sessionManager.TryGet(HttpContext.Session.Id, out var session))
            {
                return nodeid;
            }

            return NotLoggedIn;
        }

        [HttpPost("takeover/{hacksession}")]
        public string TakeOver(string hacksession)
        {
            if (sessionManager.TryGet(HttpContext.Session.Id, out var session))
            {
                return hacksession;
            }

            return NotLoggedIn;
        }

        [HttpGet("info/{nodeid}")]
        public string Info(string nodeid)
        {
            if (sessionManager.TryGet(HttpContext.Session.Id, out var session))
            {
                return nodeid;
            }

            return NotLoggedIn;
        }
    }
}