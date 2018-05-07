using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public interface IPlayerPacketHandler
    {
        Task HandlePlayerPacketAsync(UserSession userSession, Connection socket, Packet packet);
    }
}