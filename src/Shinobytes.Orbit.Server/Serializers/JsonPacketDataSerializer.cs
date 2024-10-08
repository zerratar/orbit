﻿using System.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shinobytes.Orbit.Server
{
    public class JsonPacketDataSerializer : IPacketDataSerializer
    {
        public T Serialize<T>(Packet data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data.Data, 0, data.Size);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public Packet Deserialize<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return new Packet(
                this,
                typeof(T).Name,
                bytes,
                bytes.Length,
                WebSocketMessageType.Text,
                true);
        }

        public Packet Deserialize(byte[] buffer, int offset, int size, WebSocketMessageType messageType, bool endOfMessage)
        {
            var json = System.Text.Encoding.UTF8.GetString(buffer, offset, size);

            try
            {

                var obj = JObject.Parse(json);
                var header = obj["header"].Value<string>();
                var data = System.Text.Encoding.UTF8.GetBytes(obj["data"].ToString());
                return new Packet(this, header, data, data.Length, messageType, endOfMessage);

            }
            catch
            {
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes($"\"{json}\"");
            return new Packet(this, string.Empty, bytes, bytes.Length, messageType, endOfMessage);
        }

        public byte[] Deserialize(Packet packet)
        {
            // only function to actually compress a packet to its final form
            var jsonData = System.Text.Encoding.UTF8.GetString(packet.Data, 0, packet.Size);
            return System.Text.Encoding.UTF8.GetBytes($"{{header: \"{packet.Header}\", data: {jsonData}}}");
        }
    }
}