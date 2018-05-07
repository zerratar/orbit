namespace Shinobytes.Orbit.Server
{
    public class AdminSession : UserSession
    {
        public AdminSession(string sessionId, Player player)
            : base(sessionId, player, true)
        {
        }
    }
}