using System.Net.WebSockets;

namespace Shinobytes.Orbit.Server
{
    public class ConnectionProvider : IConnectionProvider
    {
        public Connection Get(WebSocket socket)
        {
            return new Connection(socket);
        }
    }
}