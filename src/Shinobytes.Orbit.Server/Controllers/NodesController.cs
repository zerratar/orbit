using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Orbit.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NodesController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("takeover/{nodeid}")]
        public string TakeOver(string nodeid)
        {
            return nodeid;
        }

        [HttpGet("info/{nodeid}")]
        public string Info(string nodeid)
        {
            return nodeid;
        }
    }
}