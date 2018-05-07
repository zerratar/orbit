using System;

namespace Shinobytes.Orbit.Server
{
    public interface IGame
    {
        void Begin();

        void PlayerConnectionEstablished(UserSession userSession);
        void PlayerConnectionClosed(UserSession userSession);
        void PlayerPositionChanged(UserSession userSession, double latitude, double longitude);
        void PlayerPing(UserSession userSession, DateTime origin, long pid);
    }
}