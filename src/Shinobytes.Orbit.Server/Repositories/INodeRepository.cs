using System;
using System.Collections.Generic;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public interface INodeRepository
    {
        IEnumerable<Node> GetWithin(double latitude, double longitude, double radiusMeters);
    }
}