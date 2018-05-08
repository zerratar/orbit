using System;
using System.Collections.Concurrent;

namespace Shinobytes.Orbit.Server
{
    public class MemoryBasedPlayerRepository : IPlayerRepository
    {
        private readonly ConcurrentDictionary<string, Player>
            players = new ConcurrentDictionary<string, Player>();

        public MemoryBasedPlayerRepository()
        {
            players["lichine"] = new Player
            {
                Id = 0,
                IsAdmin = true,
                Level = 1,
                ViewRange = 2000,
                Username = "Lichine",
                Password = "password",
                Created = DateTime.UtcNow
            };

            players["zerratar"] = new Player
            {
                Id = 1,
                IsAdmin = true,
                Level = 1,
                ViewRange = 2000,
                Username = "Zerratar",
                Password = "password",
                Created = DateTime.UtcNow
            };

            players["user"] = new Player
            {
                Id = 2,
                IsAdmin = false,
                Level = 1,
                ViewRange = 500,
                Username = "User",
                Password = "password",
                Created = DateTime.UtcNow
            };
        }

        public Player Find(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            if (players.TryGetValue(username.ToLower(), out var player))
            {
                return player;
            }

            return null;
        }
    }
}