using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public abstract class UserSession
    {
        private readonly object sessionMutex = new object();
        private Connection connection;

        protected UserSession(string sessionId, Player player, bool isAdmin)
        {
            Player = player;
            IsAdmin = isAdmin;
            Id = sessionId;
        }

        public string Id { get; }

        public bool IsAdmin { get; }

        public Player Player { get; }

        public void SetConnection(Connection connection)
        {
            lock (sessionMutex)
            {
                this.connection = connection;
            }
        }

        public Task<T> ReceiveAsync<T>()
        {
            lock (sessionMutex)
            {
                if (this.connection == null)
                {
                    throw new NoConnectionException();
                }
                return connection.ReceiveAsync<T>();
            }
        }

        public void Send<T>(T data)
        {
            lock (sessionMutex)
            {
                if (this.connection == null)
                {
                    throw new NoConnectionException();
                }
                connection.EnqueueSend<T>(data);
            }
        }

        public void Close()
        {
            lock (sessionMutex)
            {
                if (this.connection != null)
                {
                    this.connection.Close();
                }
            }
        }

        public Task KeepAlive()
        {
            lock (sessionMutex)
            {
                return this.connection.KillTask.Task;
            }
        }
    }
}
