using System;

namespace Shinobytes.Orbit.Server
{
    public class Player
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        public double ViewRange { get; set; }

        public int Level { get; set; }
        public long Experience { get; set; }
        public long Resources { get; set; }

        public GeoCoordinate Position { get; set; }
        public DateTime PositionChanged { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime Created { get; set; }
    }
}