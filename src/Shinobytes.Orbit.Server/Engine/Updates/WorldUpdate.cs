using System.Linq;

namespace Shinobytes.Orbit.Server
{
    public class WorldUpdate : GameUpdate
    {
        private readonly INodeObserver nodeObserver;
        private readonly INodeRepository nodeRepository;

        public WorldUpdate(
            UserSession target,
            INodeObserver nodeObserver,
            INodeRepository nodeRepository)
            : base(target)
        {
            this.nodeObserver = nodeObserver;
            this.nodeRepository = nodeRepository;
        }

        public override void Update()
        {
            // observed = get-observed Target
            // added    = get-added observed, Target
            // updated  = get-updated observed, Target
            // removed  = get-removed observed, Target
            // World update includes:

            var playerInfo = new Requests.PlayerInfo(
                Target.Player.Username,
                Target.Player.Level,
                Target.Player.Experience,
                Target.Player.Resources);

            var geoBounds = Coordinates.GetBoundingBox(Target.Player.Position, Target.Player.ViewRange / 2f);
            var visibleNodes = nodeRepository.GetWithin(Target.Player.Position, Target.Player.ViewRange);
            var delta = nodeObserver.Observe(Target, visibleNodes);

            var areaNodes = new Requests.AreaNodes
            {
                Bounds = geoBounds,
                Added = delta.Added,
                Updated = delta.Updated,
                Removed = delta.Removed,
            };

            Target.Send(areaNodes);
            Target.Send(playerInfo);

            // AreaNodes
            //      Target.Send(new Requests.Nodes(added, updated, removed));            
        }
    }
}