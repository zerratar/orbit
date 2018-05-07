using System;

namespace Shinobytes.Orbit.Server
{
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
}