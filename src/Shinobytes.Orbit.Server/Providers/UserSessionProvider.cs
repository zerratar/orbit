using System.Net.WebSockets;
using System.Threading.Tasks;
using Shinobytes.Core;

namespace Shinobytes.Orbit.Server
{
    public class UserSessionProvider : IPlayerSessionProvider
    {
        private readonly ILogger logger;
        private readonly IConnectionProvider connectionProvider;
        private readonly IPlayerConnectionHandler playerConnectionHandler;
        private readonly IPlayerSessionBinder sessionBinder;

        public UserSessionProvider(
            ILogger logger,
            IConnectionProvider connectionProvider,
            IPlayerConnectionHandler playerConnectionHandler,
            IPlayerSessionBinder sessionBinder)
        {
            this.logger = logger;
            this.connectionProvider = connectionProvider;
            this.playerConnectionHandler = playerConnectionHandler;
            this.sessionBinder = sessionBinder;
        }

        public async Task<UserSession> GetAsync(WebSocket socket)
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

            logger.WriteDebug($"Session '{playerSession.Id}' connected to stream.");

            playerConnectionHandler.Open(playerSession, connection);

            return playerSession;
        }
    }
}