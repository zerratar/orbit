namespace Shinobytes.Orbit.Server
{
    public abstract class GameUpdate
    {
        protected GameUpdate(UserSession target)
        {
            this.Target = target;
        }

        public UserSession Target { get; }

        public abstract void Update();
    }
}