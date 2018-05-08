namespace Shinobytes.Orbit.Server
{
    public class NodesUpdate : GameUpdate
    {
        private readonly INodeObserver nodeObserver;
        private readonly INodeRepository nodeRepository;

        public NodesUpdate(UserSession target, INodeObserver nodeObserver, INodeRepository nodeRepository) : base(target)
        {
            this.nodeObserver = nodeObserver;
            this.nodeRepository = nodeRepository;
        }

        public override void Update()
        {
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
        }
    }
}