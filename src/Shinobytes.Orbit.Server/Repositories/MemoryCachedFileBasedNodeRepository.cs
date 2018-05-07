using System.Collections.Generic;
using System.Linq;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public class MemoryCachedFileBasedNodeRepository : MemoryBasedFileRepository<Node, string>, INodeRepository
    {
        public MemoryCachedFileBasedNodeRepository()
            : base("nodes", n => n.Id)
        {
        }

        public IEnumerable<Node> GetWithin(double latitude, double longitude, double radiusMeters)
        {
            return from node in this.All()
                   where node.WithinRadiusMeters(latitude, longitude, radiusMeters)
                   select node;
        }
    }
}