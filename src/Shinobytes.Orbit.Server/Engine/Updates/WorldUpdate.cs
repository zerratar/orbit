using System;

namespace Shinobytes.Orbit.Server
{
    public class WorldUpdate : GameUpdate
    {
        public WorldUpdate(UserSession target)
            : base(target)
        {
        }

        public override void Update()
        {
            // observed = get-observed Target
            // added    = get-added observed, Target
            // updated  = get-updated observed, Target
            // removed  = get-removed observed, Target
            // World update includes:
            //      Target.Send(new Requests.PlayerInfo(username, level, experience, resources, owned-node ids))
            //      Target.Send(new Requests.Nodes(added, updated, removed));            
        }
    }

    public class PlayerPingUpdate : GameUpdate
    {
        private readonly DateTime timestamp;
        private readonly long pid;

        public PlayerPingUpdate(UserSession target, DateTime timestamp, long pid) : base(target)
        {
            this.timestamp = timestamp;
            this.pid = pid;
        }

        public override void Update()
        {
            Target.Send(new Requests.PlayerKeepAlive
            {
                Id = pid,
                Origin = timestamp,
                Reply = DateTime.UtcNow
            });
        }
    }

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

    public class NodesUpdate : GameUpdate
    {
        public NodesUpdate(UserSession target) : base(target)
        {
        }

        public override void Update()
        {
            // observed = get-observed Target
            // added    = get-added observed, Target
            // updated  = get-updated observed, Target
            // removed  = get-removed observed, Target
            // Nodes update includes:         
            //      Target.Send(new Requests.Nodes(added, updated, removed));            
        }
    }
}