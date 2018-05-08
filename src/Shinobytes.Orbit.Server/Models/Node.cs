namespace Shinobytes.Orbit.Server.Models
{
    public class Node
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int Tier { get; set; }

        public bool WithinRadiusMeters(double originLatitude, double originLongitude, double meters)
        {
            var distance = Coordinates.Distance(originLatitude, originLongitude, Latitude, Longitude, 'K');
            var withinRadiusMeters = distance <= meters / 1000f;
            return withinRadiusMeters;
        }

        //public static float GetNodeStartingHealth(Node node)
        //{
        //    return (float)(Math.Pow(2, node.Tier) * 150f) + ((node.Types?.Length).GetValueOrDefault(1) * 75f) + node.Tier * 25f;
        //}
    }
}
