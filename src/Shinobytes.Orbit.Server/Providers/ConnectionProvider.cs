using System.Net.WebSockets;
using Shinobytes.Core;

namespace Shinobytes.Orbit.Server
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly ILogger logger;
        private readonly IPacketDataSerializer serializer;

        public ConnectionProvider(ILogger logger, IPacketDataSerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
        }

        public Connection Get(WebSocket socket)
        {
            return new Connection(logger, socket, serializer);
        }
    }
}