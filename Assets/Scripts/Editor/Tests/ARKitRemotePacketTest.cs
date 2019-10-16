using System.Runtime.InteropServices;
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
                projectionMatrix = new Matrix4x4(
                    new Vector4(1, 2, 3, 4),
                    new Vector4(5, 6, 7, 8),
                    new Vector4(9, 10, 11, 12),
                    new Vector4(13, 14, 15, 16)
                ),
                displayMatrix = new Matrix4x4(
                    new Vector4(17, 18, 19, 20),
                    new Vector4(21, 22, 23, 24),
                    new Vector4(25, 26, 27, 28),
                    new Vector4(29, 30, 31, 32)
                )
            };
            var b = new ARKitRemotePacket.CameraFrameEvent()
            {
                timestampNs = 123,
                projectionMatrix = new Matrix4x4(
                    new Vector4(1, 2, 3, 4),
                    new Vector4(5, 6, 7, 8),
                    new Vector4(9, 10, 11, 12),
                    new Vector4(13, 14, 15, 16)
                ),
                displayMatrix = new Matrix4x4(
                    new Vector4(17, 18, 19, 20),
                    new Vector4(21, 22, 23, 24),
                    new Vector4(25, 26, 27, 28),
                    new Vector4(29, 30, 31, 32)
                )
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
                    projectionMatrix = new Matrix4x4(
                        new Vector4(1, 2, 3, 4),
                        new Vector4(5, 6, 7, 8),
                        new Vector4(9, 10, 11, 12),
                        new Vector4(13, 14, 15, 16)
                    ),
                    displayMatrix = new Matrix4x4(
                        new Vector4(17, 18, 19, 20),
                        new Vector4(21, 22, 23, 24),
                        new Vector4(25, 26, 27, 28),
                        new Vector4(29, 30, 31, 32)
                    )
                }
            };
            var data = a.Serialize();
            var b = ARKitRemotePacket.Deserialize(data);

            Assert.AreEqual(a.cameraFrame, b.cameraFrame);


            var c = new ARKitRemotePacket()
            {
                cameraFrame = new ARKitRemotePacket.CameraFrameEvent()
                {
                    timestampNs = 123,
                    projectionMatrix = new Matrix4x4(
                        new Vector4(1, 2, 3, 4),
                        new Vector4(5, 6, 7, 8),
                        new Vector4(9, 10, 11, 12),
                        new Vector4(13, 14, 15, 16)
                    ),
                    displayMatrix = new Matrix4x4(
                        new Vector4(17, 18, 19, 20),
                        new Vector4(21, 22, 23, 24),
                        new Vector4(25, 26, 27, 28),
                        new Vector4(29, 30, 31, 32)
                    )
                }
            };

            var data2 = a.Serialize();
            var d = ARKitRemotePacket.Deserialize(data2);

            Assert.AreEqual(c.cameraFrame, d.cameraFrame);

        }
    }
}