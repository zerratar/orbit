using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public class MemoryCachedFileBasedNodeRepository : MemoryBasedFileRepository<Node, string>, INodeRepository
    {
        public MemoryCachedFileBasedNodeRepository(INodeChangeTracker nodeChangeTracker)
            : base("nodes", nodeChangeTracker, n => n.Id)
        {
        }

        public IEnumerable<Node> GetWithin(GeoCoordinate coords, double radiusMeters)
        {
            return GetWithin(coords.Latitude, coords.Longitude, radiusMeters);
        }

        public IEnumerable<Node> GetWithin(double latitude, double longitude, double radiusMeters)
        {
            return from node in this.All()
                   where node.WithinRadiusMeters(latitude, longitude, radiusMeters)
                   select node;
        }
    }
}