using UnityEngine;
using NUnit.Framework;


namespace ARKitStream.Internal
{
    public class ARKitRemotePacketTest
    {
        [Test]
        public static void CameraFrameEventEqual()
        {
            var a = new ARKitRemotePacket.CameraFrameEvent()
            {
                timestampNs = 123,
                projectionMatrix = TestUtil.MockMatrix(1),
                displayMatrix = TestUtil.MockMatrix(17),
            };
            var b = new ARKitRemotePacket.CameraFrameEvent()
            {
                timestampNs = 123,
                projectionMatrix = TestUtil.MockMatrix(1),
                displayMatrix = TestUtil.MockMatrix(17),
            };
            Assert.AreEqual(a, b);
        }

        [Test]
        public static void SerializeDeserialize()
        {
            var a = new ARKitRemotePacket()
            {
                cameraFrame = new ARKitRemotePacket.CameraFrameEvent()
                {
                    timestampNs = 123,
                    projectionMatrix = TestUtil.MockMatrix(1),
                    displayMatrix = TestUtil.MockMatrix(17),
                }
            };
            var data = a.Serialize();
            var b = ARKitRemotePacket.Deserialize(data);

            Assert.AreEqual(a.cameraFrame, b.cameraFrame);

            Debug.Log("binary size is : " + data.Length);

            var c = new ARKitRemotePacket()
            {
                cameraFrame = new ARKitRemotePacket.CameraFrameEvent()
                {
                    timestampNs = 123,
                    projectionMatrix = TestUtil.MockMatrix(1),
                    displayMatrix = TestUtil.MockMatrix(17),
                }
            };

            var data2 = a.Serialize();
            var d = ARKitRemotePacket.Deserialize(data2);

            Assert.AreEqual(c.cameraFrame, d.cameraFrame);

        }
    }
}