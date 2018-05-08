using System;
using System.Collections.Generic;

namespace Shinobytes.Orbit.Server
{
    public class NodeObservation
    {
        public Dictionary<string, DateTime> Nodes { get; } = new Dictionary<string, DateTime>();
    }
}