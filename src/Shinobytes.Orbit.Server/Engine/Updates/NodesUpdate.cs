namespace Shinobytes.Orbit.Server
{
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