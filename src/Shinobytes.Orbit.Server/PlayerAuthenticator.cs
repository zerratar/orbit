namespace Shinobytes.Orbit.Server
{
    public class PlayerAuthenticator : IPlayerAuthenticator
    {
        private readonly IPlayerRepository playerRepository;

        public PlayerAuthenticator(IPlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
        }

        public Player Authenticate(string username, string password)
        {
            return null;
        }
    }
}