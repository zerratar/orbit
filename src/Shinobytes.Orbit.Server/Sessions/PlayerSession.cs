namespace Shinobytes.Orbit.Server
{
    public class PlayerSession : UserSession
    {
        public PlayerSession(string sessionId, Player player)
            : base(sessionId, player, false)
        {
        }
    }
}