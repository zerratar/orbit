using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public interface IPlayerSessionBinder
    {
        // bind session id to connection to associate a socket with a user that has logged in.

        Task<PlayerSession> BindAsync(Connection connection);
    }
}