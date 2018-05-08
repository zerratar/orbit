using System;
using System.Collections.Concurrent;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public class NodeChangeTracker : INodeChangeTracker
    {
        private readonly ConcurrentDictionary<string, DateTime> nodes
            = new ConcurrentDictionary<string, DateTime>();

        public void Update(Node node)
        {
            nodes[node.Id] = DateTime.UtcNow;
        }

        public DateTime GetChangeTime(Node node)
        {
            if (nodes.TryGetValue(node.Id, out var dt))
                return dt;

            return DateTime.MinValue;
        }
    }
}