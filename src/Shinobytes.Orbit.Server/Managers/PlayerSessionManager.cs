namespace Shinobytes.Orbit.Server
{
    public class PlayerSessionManager : IPlayerSessionManager
    {
        public bool TryGet(string token, out PlayerSession playerSession)
        {
            playerSession = null;
            return false;
        }

        public bool TryGetByUsername(string username, out PlayerSession playerSession)
        {
            playerSession = null;
            return false;
        }

        public void EndSession(PlayerSession playerSession)
        {
        }

        public string BeginSession(string sessionId, Player player)
        {
            return null;
        }
    }
}