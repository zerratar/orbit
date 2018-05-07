using System;

namespace Shinobytes.Orbit.Server
{
    public struct PlayerPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public DateTime Changed { get; set; }

        public PlayerPosition(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Changed = DateTime.UtcNow;
        }
    }
}