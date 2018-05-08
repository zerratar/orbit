using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public class PlayerConnectionHandler : IPlayerConnectionHandler
    {
        private readonly IGame game;
        private readonly IPlayerPacketHandler packetHandler;
        private readonly ConcurrentDictionary<string, Thread> writeThreads
            = new ConcurrentDictionary<string, Thread>();

        private readonly ConcurrentDictionary<string, Thread> readThreads
            = new ConcurrentDictionary<string, Thread>();

        public PlayerConnectionHandler(IGame game, IPlayerPacketHandler packetHandler)
        {
            this.game = game;
            this.packetHandler = packetHandler;
        }

        public void Open(UserSession userSession, Connection socket)
        {

            // start 1 read/write thread per connection, for now. and then keep each connection single threaded.

            game.PlayerConnectionEstablished(userSession);
            writeThreads[userSession.Id] = new Thread(async () => await PlayerConnectionWrite(userSession, socket));
            writeThreads[userSession.Id].Start();

            readThreads[userSession.Id] = new Thread(async () => await PlayerConnectionRead(userSession, socket));
            readThreads[userSession.Id].Start();
        }

        private async Task PlayerConnectionWrite(UserSession userSession, Connection socket)
        {
            do
            {
                await ProcessSendQueueAsync(socket);

            } while (!socket.Closed);
        }

        private async Task PlayerConnectionRead(UserSession userSession, Connection socket)
        {
            do
            {
                await HandlePacketsAsync(userSession, socket); // fire and forget

            } while (!socket.Closed);
            game.PlayerConnectionClosed(userSession);
        }

        private async Task HandlePacketsAsync(UserSession userSession, Connection socket)
        {
            var result = await socket.ReceiveAsync();
            if (result == null)
            {
                return;
            }

            await HandlePacketAsync(userSession, socket, result);
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

            if (writeThreads.TryGetValue(userSession.Id, out var writeThread))
            {
                writeThread.Join();
            }

            if (readThreads.TryGetValue(userSession.Id, out var readThread))
            {
                readThread.Join();
            }
        }
    }
}