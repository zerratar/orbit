using System.Collections.Generic;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public class NodeObserverChanges
    {
        public NodeObserverChanges(List<Node> added, List<Node> updated, List<string> removed)
        {
            Added = added;
            Updated = updated;
            Removed = removed;
        }

        public List<Node> Added { get; }
        public List<Node> Updated { get; }
        public List<string> Removed { get; }
    }
}