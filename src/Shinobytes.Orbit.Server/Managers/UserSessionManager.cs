using System;
using System.Collections.Concurrent;

namespace Shinobytes.Orbit.Server
{
    public class UserSessionManager : IUserSessionManager
    {
        private readonly ConcurrentDictionary<string, UserSession> sessions
            = new ConcurrentDictionary<string, UserSession>();

        public bool TryGet(string token, out UserSession userSession)
        {
            return sessions.TryGetValue(token, out userSession);
        }

        public bool TryGetByUsername(string username, out UserSession userSession)
        {
            var sessionKeys = sessions.Keys;
            foreach (var key in sessionKeys)
            {
                if (sessions.TryGetValue(key, out userSession)
                    && userSession.Player.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            userSession = null;
            return false;
        }

        public void EndSession(UserSession userSession)
        {
            sessions.TryRemove(userSession.Id, out _);
        }

        public string BeginSession(string sessionId, Player player)
        {
            UserSession session;
            if (player.IsAdmin)
            {
                session = new AdminSession(sessionId, player);
            }
            else
            {
                session = new PlayerSession(sessionId, player);
            }

            sessions[sessionId] = session;
            return sessionId;
        }
    }
}