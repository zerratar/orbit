﻿namespace Shinobytes.Orbit.Server
{
    public interface IGame
    {
        void Begin();

        void PlayerConnectionEstablished(PlayerSession playerSession);
    }
}