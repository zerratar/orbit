using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shinobytes.Orbit.Server
{
    public interface IPlayerConnectionHandler
    {
        void Open(PlayerSession playerSession, Connection socket);
    }

    public class PlayerConnectionHandler : IPlayerConnectionHandler
    {
        private readonly IPlayerConnectionHandlerSettings settings;

        public PlayerConnectionHandler(IPlayerConnectionHandlerSettings settings)
        {
            this.settings = settings;
        }

        public void Open(PlayerSession playerSession, Connection socket)
        {
            // start 1 read/write thread per connection, for now. and then keep each connection single threaded.

        }

        public void Close(PlayerSession playerSession)
        {
            playerSession.Close();
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}