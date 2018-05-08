using System;

namespace Shinobytes.Orbit.Server
{
    public static class Coordinates
    {
        private const double DegreesToRadians = Math.PI / 180.0;
        private const double RadiansToDegrees = 180.0 / Math.PI;
        private const double EarthRadius = 6378137.0;

        // Semi-axes of WGS-84 geoidal reference
        private const double WGS84_a = 6378137.0; // Major semiaxis [m]
        private const double WGS84_b = 6356752.3; // Minor semiaxis [m]


        public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = dist * 60 * 1.1515;
            switch (unit)
            {
                case 'K':
                    dist = dist * 1.609344;
                    break;
                case 'N':
                    dist = dist * 0.8684;
                    break;
            }
            return dist;
        }

        public static GeoCoordinate Offset(GeoCoordinate coordinate, double meters)
        {
            return Offset(coordinate.Latitude, coordinate.Longitude, meters);
        }

        public static GeoCoordinate Offset(double latitude, double longitude, double meters)
        {
            // number of km per degree = ~111km (111.32 in google maps, but range varies between 110.567km at the equator and 111.699km at the poles)
            // 1km in degree = 1 / 111.32km = 0.0089
            // 1m in degree = 0.0089 / 1000 = 0.0000089
            var coef = meters * 0.0000089;

            var newLat = latitude + coef;

            // pi / 180 = 0.018
            var newLong = longitude + coef / Math.Cos(latitude * 0.018);

            return new GeoCoordinate()
            {
                Latitude = newLat,
                Longitude = newLong
            };
        }

        public static GeoCoordinate Offset(GeoCoordinate coordinate, double metersNorth, double metersEast)
        {
            double lat = coordinate.Latitude;
            double lon = coordinate.Longitude;
            //Earth’s radius, choose the one that fits your needs 
            //final int r=6378137; final int r=6372814;
            double r = 6378137.0;

            //offsets in meters 
            double dn = metersNorth;
            double de = metersEast;
            //Coordinate offsets in radians 
            double dLat = dn / r;
            double dLon = de / (r * Math.Cos(Math.PI * lat / 180));
            //OffsetPosition, decimal degrees 
            double latO = lat + dLat * 180 / Math.PI;
            double lonO = lon + dLon * 180 / Math.PI;

            return new GeoCoordinate(latO, lonO, 0);
        }

        /// <summary>
        /// Calculates the end-point from a given source at a given range (meters) and bearing (degrees).
        /// This methods uses simple geometry equations to calculate the end-point.
        /// </summary>
        /// <param name="source">Point of origin</param>
        /// <param name="range">Range in meters</param>
        /// <param name="bearing">Bearing in degrees</param>
        /// <returns>End-point from the source given the desired range and bearing.</returns>
        public static GeoCoordinate CalculateDerivedPosition(this GeoCoordinate source, double range, double bearing)
        {

            /* Example:
             -------------------------------------------
                var center = new GeoCoordinate(0, 0);
                var radius = 111320;
                var southBound = center.CalculateDerivedPosition(radius, -180);
                var westBound = center.CalculateDerivedPosition(radius, -90);
                var eastBound = center.CalculateDerivedPosition(radius, 90);
                var northBound = center.CalculateDerivedPosition(radius, 0);
             */

            var latA = source.Latitude * DegreesToRadians;
            var lonA = source.Longitude * DegreesToRadians;
            var angularDistance = range / EarthRadius;
            var trueCourse = bearing * DegreesToRadians;

            var lat = Math.Asin(
                Math.Sin(latA) * Math.Cos(angularDistance) +
                Math.Cos(latA) * Math.Sin(angularDistance) * Math.Cos(trueCourse));

            var dlon = Math.Atan2(
                Math.Sin(trueCourse) * Math.Sin(angularDistance) * Math.Cos(latA),
                Math.Cos(angularDistance) - Math.Sin(latA) * Math.Sin(lat));

            var lon = ((lonA + dlon + Math.PI) % (Math.PI * 2)) - Math.PI;

            return new GeoCoordinate(
                lat * RadiansToDegrees,
                lon * RadiansToDegrees,
                source.Altitude);
        }

        // 'halfSideInKm' is the half length of the bounding box you want in kilometers.
        public static GeoBounds GetBoundingBox(GeoCoordinate point, double halfSideInKm)
        {
            // Bounding box surrounding the point at given coordinates,
            // assuming local approximation of Earth surface as a sphere
            // of radius given by WGS84
            var lat = Deg2Rad(point.Latitude);
            var lon = Deg2Rad(point.Longitude);
            var halfSide = 1000 * halfSideInKm;

            // Radius of Earth at given latitude
            var radius = WGS84EarthRadius(lat);
            // Radius of the parallel at given latitude
            var pradius = radius * Math.Cos(lat);

            var latMin = lat - halfSide / radius;
            var latMax = lat + halfSide / radius;
            var lonMin = lon - halfSide / pradius;
            var lonMax = lon + halfSide / pradius;

            return new GeoBounds
            {
                Min = new GeoCoordinate { Latitude = Rad2Deg(latMin), Longitude = Rad2Deg(lonMin) },
                Max = new GeoCoordinate { Latitude = Rad2Deg(latMax), Longitude = Rad2Deg(lonMax) },
                Center = point
            };
        }

        // Earth radius at a given latitude, according to the WGS-84 ellipsoid [m]
        private static double WGS84EarthRadius(double lat)
        {
            // http://en.wikipedia.org/wiki/Earth_radius
            var An = WGS84_a * WGS84_a * Math.Cos(lat);
            var Bn = WGS84_b * WGS84_b * Math.Sin(lat);
            var Ad = WGS84_a * Math.Cos(lat);
            var Bd = WGS84_b * Math.Sin(lat);
            return Math.Sqrt((An * An + Bn * Bn) / (Ad * Ad + Bd * Bd));
        }

        private static double Deg2Rad(double deg) => deg * Math.PI / 180.0;
        private static double Rad2Deg(double rad) => rad / Math.PI * 180.0;

    }
}