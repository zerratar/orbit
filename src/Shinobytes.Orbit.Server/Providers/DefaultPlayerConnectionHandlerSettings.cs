namespace Shinobytes.Orbit.Server
{
    public class DefaultPlayerConnectionHandlerSettings : IPlayerConnectionHandlerSettings
    {
        public int PacketReadSize { get; } = 4096;
    }
}