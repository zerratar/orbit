namespace Shinobytes.Orbit.Server
{
    public class PlayerUpdate : GameUpdate
    {
        public PlayerUpdate(UserSession target) : base(target)
        {
        }

        public override void Update()
        {
            // Player update includes:
            //      Target.Send(new Requests.PlayerInfo(username, level, experience, resources, owned-node ids))       
        }
    }
}