namespace Shinobytes.Orbit.Server
{
    public struct GeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public GeoCoordinate(double latitude, double longitude, double altitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
        }

        public static double operator -(GeoCoordinate coords1, GeoCoordinate coords2)
        {
            return Coordinates.Distance(coords1.Latitude, coords1.Longitude, coords2.Latitude, coords2.Longitude, 'K');
        }

        public static GeoCoordinate operator +(GeoCoordinate coords1, double meters)
        {
            return Coordinates.Offset(coords1, meters);
        }

        public static GeoCoordinate operator -(GeoCoordinate coords1, double meters)
        {
            return Coordinates.Offset(coords1, -meters);
        }
    }
}