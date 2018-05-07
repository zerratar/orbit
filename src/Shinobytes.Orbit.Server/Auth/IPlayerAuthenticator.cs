namespace Shinobytes.Orbit.Server
{
    public interface IPlayerAuthenticator
    {
        Player Authenticate(string username, string password);
    }
}