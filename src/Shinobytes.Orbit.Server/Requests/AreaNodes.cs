using System.Collections.Generic;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server.Requests
{
    public class AreaNodes
    {
        public AreaNodes()
        {

        }

        public GeoBounds Bounds { get; set; }
        public List<Node> Added { get; set; }
        public List<Node> Updated { get; set; }
        public List<string> Removed { get; set; }
    }
}