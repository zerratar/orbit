using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shinobytes.Orbit.Server
{
    public abstract class PlayerSession
    {
        private readonly object sessionMutex = new object();
        private Connection connection;

        public string Id { get; protected set; }

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
    }
}
