using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shinobytes.Orbit.Server.Requests;

namespace Shinobytes.Orbit.Server.Tests
{
    [TestClass]
    public class PacketSerializationTests
    {
        [TestMethod]
        public void DeserializeAndSerializePacket_same_packet_data()
        {
            var serializer = new JsonPacketDataSerializer();
            var posUpdate = new PlayerPositionUpdate();
            posUpdate.Latitude = 125;
            posUpdate.Longitude = 99.5;

            var packet = serializer.Deserialize(posUpdate);
            var posUpdateSerialized = serializer.Serialize<PlayerPositionUpdate>(packet);
            Assert.AreEqual(posUpdate.Latitude, posUpdateSerialized.Latitude);
            Assert.AreEqual(posUpdate.Longitude, posUpdateSerialized.Longitude);
        }

        [TestMethod]
        public void Test_binary_deserialization()
        {
            var serializer = new JsonPacketDataSerializer();
            var posUpdate = new PlayerPositionUpdate();
            posUpdate.Latitude = 125;
            posUpdate.Longitude = 99.5;

            var packet = serializer.Deserialize(posUpdate);
            var binary = serializer.Deserialize(packet);
            var data = System.Text.Encoding.UTF8.GetString(binary);
            Assert.AreEqual("{header: \"PlayerPositionUpdate\", data: {\"Latitude\":125.0,\"Longitude\":99.5}}", data);
        }
    }
}
