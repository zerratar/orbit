using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public class PlayerSessionProvider : IPlayerSessionProvider
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly IPlayerSessionBinder sessionBinder;

        public PlayerSessionProvider(IConnectionProvider connectionProvider, IPlayerSessionBinder sessionBinder)
        {
            this.connectionProvider = connectionProvider;
            this.sessionBinder = sessionBinder;
        }

        public async Task<PlayerSession> GetAsync(WebSocket socket)
        {
            var connection = connectionProvider.Get(socket);
            if (connection == null)
            {
                throw new BadConnectionException();
            }
            var playerSession = await sessionBinder.BindAsync(connection);
            if (playerSession == null)
            {
                throw new SessionNotFoundException();
            }
            return playerSession;
        }
    }
}