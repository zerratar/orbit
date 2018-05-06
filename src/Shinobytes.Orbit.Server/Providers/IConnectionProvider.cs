using System.Net.WebSockets;

namespace Shinobytes.Orbit.Server
{
    public interface IConnectionProvider
    {
        Connection Get(WebSocket socket);
    }
}