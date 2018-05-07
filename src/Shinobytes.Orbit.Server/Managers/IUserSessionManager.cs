namespace Shinobytes.Orbit.Server
{
    public interface IUserSessionManager
    {
        bool TryGet(string token, out UserSession userSession);
        bool TryGetByUsername(string username, out UserSession userSession);
        void EndSession(UserSession userSession);
        string BeginSession(string sessionId, Player player);
    }
}