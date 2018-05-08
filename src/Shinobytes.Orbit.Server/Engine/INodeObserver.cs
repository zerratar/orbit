using System.Collections.Generic;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public interface INodeObserver
    {
        NodeObserverChanges Observe(UserSession session, IEnumerable<Node> currentlyVisibleNodes);
    }
}