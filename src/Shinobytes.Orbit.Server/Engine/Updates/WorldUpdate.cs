﻿namespace Shinobytes.Orbit.Server
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

            Target.Send(new Requests.PlayerInfo(Target.Player.Username, Target.Player.Level, Target.Player.Experience,
                Target.Player.Resources));
            //      Target.Send(new Requests.Nodes(added, updated, removed));            
        }
    }
}