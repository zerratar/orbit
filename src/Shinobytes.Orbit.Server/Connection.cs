using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public class Connection
    {
        private readonly WebSocket socket;

        public Connection(WebSocket socket)
        {
            this.socket = socket;
        }

        public Task<T> ReceiveAsync<T>()
        {
            return default(Task<T>);
        }

        public void Close()
        {
            if (this.socket.State == WebSocketState.Connecting 
                || this.socket.State == WebSocketState.Open 
                || this.socket.State == WebSocketState.None)
            {
                this.socket.CloseAsync(
                    WebSocketCloseStatus.Empty, 
                    "Connection closed by server.",
                    CancellationToken.None);
            }
        }
    }
}