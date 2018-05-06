using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public interface IPlayerSessionProvider
    {
        Task<PlayerSession> GetAsync(WebSocket socket);
    }
}