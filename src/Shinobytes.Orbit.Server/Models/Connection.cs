﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Shinobytes.Core;

namespace Shinobytes.Orbit.Server
{
    public class Connection
    {
        private readonly ILogger logger;
        private readonly WebSocket socket;
        private readonly IPacketDataSerializer packetDataSerializer;
        private readonly byte[] buffer;
        private bool closed;

        public Connection(ILogger logger, WebSocket socket, IPacketDataSerializer packetDataSerializer)
        {
            this.SendQueue = new ConcurrentQueue<Packet>();
            this.logger = logger;
            this.socket = socket;
            this.packetDataSerializer = packetDataSerializer;
            this.buffer = new byte[4096];
            this.KillTask = new TaskCompletionSource<object>();
        }

        public ConcurrentQueue<Packet> SendQueue { get; }

        public bool Closed => closed || this.socket.CloseStatus.HasValue;
        public TaskCompletionSource<object> KillTask { get; set; }

        public async Task<T> ReceiveAsync<T>()
        {
            var packet = await ReceiveAsync();
            if (packet != null)
            {
                return packetDataSerializer.Serialize<T>(packet);
            }

            return default(T);
        }

        public void Close()
        {
            this.KillTask.SetResult(null);
            this.closed = true;
            if (this.socket.State == WebSocketState.Connecting
                || this.socket.State == WebSocketState.Open
                || this.socket.State == WebSocketState.None)
            {
                try
                {
                    this.socket.CloseAsync(
                        WebSocketCloseStatus.Empty,
                        "Connection closed by server.",
                        CancellationToken.None);
                }
                catch (Exception exc)
                {
                    logger.WriteError(exc.ToString());
                }
            }
        }

        public void EnqueueSend<T>(T data)
        {
            if (Closed)
            {
                return;
            }

            SendQueue.Enqueue(packetDataSerializer.Deserialize<T>(data));
        }

        public Task SendAsync<T>(T data)
        {
            return SendAsync(packetDataSerializer.Deserialize<T>(data));
        }

        public async Task SendAsync(Packet packet)
        {
            if (Closed)
            {
                return;
            }

            try
            {
                await socket.SendAsync(packet.Build(), packet.MessageType, packet.EndOfMessage, CancellationToken.None);
            }
            catch (Exception exc)
            {
                logger.WriteError(exc.ToString());
                this.Close();
            }
        }

        internal async Task<Packet> ReceiveAsync()
        {
            if (Closed)
            {
                return null;
            }

            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.CloseStatus.HasValue)
            {
                this.closed = true;
                await this.socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                return null;
            }

            return packetDataSerializer.Deserialize(buffer, 0, result.Count, result.MessageType, result.EndOfMessage);
        }
    }
}