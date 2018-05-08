using System;
using Shinobytes.Orbit.Server.Models;

namespace Shinobytes.Orbit.Server
{
    public interface INodeChangeTracker : IChangeTracker<Node>
    {
    }

    public interface IChangeTracker<T>
    {
        void Update(T node);
        DateTime GetChangeTime(T node);
    }
}