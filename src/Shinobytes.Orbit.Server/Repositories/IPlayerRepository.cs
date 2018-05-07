namespace Shinobytes.Orbit.Server
{
    public interface IPlayerRepository
    {
        Player Find(string username);
    }
}