using System;

namespace Shinobytes.Orbit.Server
{
    public class Player
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Position Position { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime Created { get; set; }
    }

    public struct Position
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
    }
}