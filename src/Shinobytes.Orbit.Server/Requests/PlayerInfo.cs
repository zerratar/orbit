namespace Shinobytes.Orbit.Server.Requests
{
    public class PlayerInfo
    {
        public PlayerInfo(string username, int level, long experience, long resources)
        {
            Username = username;
            Level = level;
            Experience = experience;
            Resources = resources;
        }

        public string Username { get; set; }
        public int Level { get; set; }
        public long Experience { get; set; }
        public long Resources { get; set; }
    }
}