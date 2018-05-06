using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public class PlayerSessionBinder : IPlayerSessionBinder
    {
        private readonly IPlayerSessionManager sessionManager;

        public PlayerSessionBinder(IPlayerSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public async Task<PlayerSession> BindAsync(Connection connection)
        {
            var token = await connection.ReceiveAsync<string>();

            if (sessionManager.TryGet(token, out var session))
            {
                session.SetConnection(connection);
                return session;
            }
            // no such session exists, user not logged in. 
            return null;
        }
    }
}