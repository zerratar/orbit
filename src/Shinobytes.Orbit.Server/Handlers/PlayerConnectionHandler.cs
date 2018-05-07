using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public class PlayerConnectionHandler : IPlayerConnectionHandler
    {
        private readonly IGame game;
        private readonly IPlayerPacketHandler packetHandler;
        private readonly ConcurrentDictionary<string, Thread> connectionThreads
            = new ConcurrentDictionary<string, Thread>();

        public PlayerConnectionHandler(IGame game, IPlayerPacketHandler packetHandler)
        {
            this.game = game;
            this.packetHandler = packetHandler;
        }

        public void Open(UserSession userSession, Connection socket)
        {
            // start 1 read/write thread per connection, for now. and then keep each connection single threaded.
            connectionThreads[userSession.Id] = new Thread(() => PlayerConnectionLoop(userSession, socket));
            connectionThreads[userSession.Id].Start();
        }

        private async void PlayerConnectionLoop(UserSession userSession, Connection socket)
        {
            game.PlayerConnectionEstablished(userSession);
            do
            {
                var result = await socket.ReceiveAsync();
                if (result == null)
                {
                    break;
                }

                await HandlePacketAsync(userSession, socket, result);
                await ProcessSendQueueAsync(socket);
            } while (!socket.Closed);
            game.PlayerConnectionClosed(userSession);
        }

        private async Task ProcessSendQueueAsync(Connection socket)
        {
            while (socket.SendQueue.TryDequeue(out var packet))
            {
                await socket.SendAsync(packet);
            }
        }

        private Task HandlePacketAsync(UserSession userSession, Connection socket, Packet packet)
        {
            return packetHandler.HandlePlayerPacketAsync(userSession, socket, packet);
        }

        public void Close(UserSession userSession)
        {
            userSession.Close();
            if (connectionThreads.TryGetValue(userSession.Id, out var thread))
            {
                thread.Join();
            }
        }
    }
}