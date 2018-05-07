using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace Shinobytes.Orbit.Server
{
    public interface IPlayerConnectionHandler
    {
        void Open(UserSession userSession, Connection socket);
    }
}