using System.Threading.Tasks;
using Shinobytes.Orbit.Server.Requests;

namespace Shinobytes.Orbit.Server
{
    public class PlayerPacketHandler : IPlayerPacketHandler
    {
        private readonly IGame game;

        public PlayerPacketHandler(IGame game)
        {
            this.game = game;
        }

        public Task HandlePlayerPacketAsync(UserSession userSession, Connection socket, Packet packet)
        {
            if (packet.Is<PlayerPositionUpdate>(out var posUpdate))
            {
                this.game.PlayerPositionChanged(userSession, posUpdate.Latitude, posUpdate.Longitude);

            }
            else if (packet.Is<PlayerKeepAlive>(out var ping))
            {
                this.game.PlayerPing(userSession, ping.Origin, ping.Id);
            }
            return null;
        }
    }
}