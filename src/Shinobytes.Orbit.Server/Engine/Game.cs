using System;
using System.Collections.Concurrent;
using System.Threading;
using Shinobytes.Core;

namespace Shinobytes.Orbit.Server
{
    public class Game : IGame
    {
        private readonly ILogger logger;
        private readonly INodeRepository nodeRepository;
        private readonly INodeObserver nodeObserver;

        private readonly ConcurrentQueue<GameUpdate> updateQueue
            = new ConcurrentQueue<GameUpdate>();

        private Thread gameThread;

        public Game(ILogger logger, INodeRepository nodeRepository, INodeObserver nodeObserver)
        {
            this.logger = logger;
            this.nodeRepository = nodeRepository;
            this.nodeObserver = nodeObserver;
        }

        public void Begin()
        {
            // start a new thread that will forever take all updates in updatequeue and process
            gameThread = new Thread(GameProcessLoop);
            gameThread.Start();
        }

        private void GameProcessLoop()
        {
            while (true)
            {
                ProcessUpdateQueue();
            }
        }

        private void ProcessUpdateQueue()
        {
            if (updateQueue.IsEmpty)
            {
                System.Threading.Thread.Sleep(10);
            }
            else
            {
                while (updateQueue.TryDequeue(out var playerUpdater))
                {
                    playerUpdater.Update();
                }
            }
        }

        public void PlayerConnectionEstablished(UserSession userSession)
        {
            if (userSession.IsAdmin)
            {
                logger.WriteDebug($"Admin '{userSession.Id}' is ready to administrate!");
            }
            else
            {
                logger.WriteDebug($"Player '{userSession.Id}' is ready for some nodes!");
            }

            // register player handler
            EnqueueWorldUpdate(userSession);
        }

        public void PlayerConnectionClosed(UserSession userSession)
        {
            logger.WriteDebug($"Oh shoot! Player '{userSession.Id}' disconnected from node feed.");
            // unregister player handler

            // todo: logout after 10 seconds of inactivity.
        }

        public void PlayerPositionChanged(UserSession userSession, double latitude, double longitude)
        {
            var position = userSession.Player.Position;
            logger.WriteDebug($"Player '{userSession.Id}' moved from lat: {position.Latitude}, lon: {position.Longitude} to lat: {latitude}, lon: {longitude}.");

            // may need to lock this to prevent it from being used in the world update logic.
            userSession.Player.Position = new GeoCoordinate(latitude, longitude, position.Altitude);

            // check if we got any new nodes visible for this player, if we do. send the changes
            // ...
            EnqueueNodesUpdate(userSession);
        }

        public void PlayerPing(UserSession userSession, DateTime timestamp, long pid)
        {
            logger.WriteDebug($"Player '{userSession.Id}' ping with id: {pid}.");
            EnqueuePlayerPing(userSession, timestamp, pid);
        }

        private void EnqueuePlayerPing(UserSession userSession, DateTime timestamp, long pid)
        {
            this.updateQueue.Enqueue(new PlayerPingUpdate(userSession, timestamp, pid));
        }
        private void EnqueuePlayerUpdate(UserSession userSession)
        {
            this.updateQueue.Enqueue(new PlayerUpdate(userSession));
        }
        private void EnqueueNodesUpdate(UserSession userSession)
        {
            this.updateQueue.Enqueue(new NodesUpdate(userSession, nodeObserver, nodeRepository));
        }
        private void EnqueueWorldUpdate(UserSession userSession)
        {
            this.updateQueue.Enqueue(new WorldUpdate(userSession, nodeObserver, nodeRepository));
        }
    }
}
