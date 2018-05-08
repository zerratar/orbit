using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public interface INodeRepository
    {
        IEnumerable<Node> GetWithin(GeoCoordinate coords, double radiusMeters);
        IEnumerable<Node> GetWithin(double latitude, double longitude, double radiusMeters);
        void Store(Node node);
        Node Get(string id);
    }
}