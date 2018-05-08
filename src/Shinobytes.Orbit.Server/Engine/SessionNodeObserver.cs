using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public class SessionNodeObserver : INodeObserver
    {
        private readonly INodeChangeTracker nodeChangeTracker;

        private readonly ConcurrentDictionary<string, NodeObservation> sessionNodes
            = new ConcurrentDictionary<string, NodeObservation>();

        public SessionNodeObserver(INodeChangeTracker nodeChangeTracker)
        {
            this.nodeChangeTracker = nodeChangeTracker;
        }

        public NodeObserverChanges Observe(UserSession session, IEnumerable<Node> currentlyVisibleNodes)
        {
            var visibleNodes = currentlyVisibleNodes.ToList();
            if (!sessionNodes.TryGetValue(session.Id, out var observation))
            {
                observation = new NodeObservation();
                foreach (var node in visibleNodes)
                {
                    observation.Nodes[node.Id] = nodeChangeTracker.GetChangeTime(node);
                }

                sessionNodes[session.Id] = observation;
                return new NodeObserverChanges(visibleNodes, new List<Node>(), new List<string>());
            }

            var added = new List<Node>();
            var updated = new List<Node>();
            var removed = new List<string>();

            // remove all first
            foreach (var node in observation.Nodes)
            {
                removed.Add(node.Key);
            }

            foreach (var node in visibleNodes)
            {
                var changeTime = nodeChangeTracker.GetChangeTime(node);
                if (!observation.Nodes.ContainsKey(node.Id))
                {
                    observation.Nodes[node.Id] = changeTime;
                    added.Add(node);
                }
                else
                {
                    if (changeTime > observation.Nodes[node.Id])
                        updated.Add(node);

                    // keep this node as it was updated.
                    removed.Remove(node.Id);
                }

            }
            return new NodeObserverChanges(added, updated, removed);
        }
    }
}