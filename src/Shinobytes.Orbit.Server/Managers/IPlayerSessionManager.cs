namespace Shinobytes.Orbit.Server
{
    public interface IPlayerSessionManager
    {
        bool TryGet(string token, out PlayerSession playerSession);
        bool TryGetByUsername(string username, out PlayerSession playerSession);
        void EndSession(PlayerSession playerSession);
        string BeginSession(string sessionId, Player player);
    }
}