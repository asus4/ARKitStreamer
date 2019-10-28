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
                },
                face = new ARKitRemotePacket.FaceInfo()
                {
                    added = new ARKitStream.Internal.XRFace[] {
                        new ARKitStream.Internal.XRFace()
                    },
                    updated = new ARKitStream.Internal.XRFace[] {

                    },
                    removed = new ARKitStream.Internal.XRFace[] {

                    },
                    meshes = new ARKitRemotePacket.FaceMesh[]
                    {
                        new ARKitRemotePacket.FaceMesh()
                        {
                            vertices = TestUtil.MockFloat3Array(10, 0),
                            normals = TestUtil.MockFloat3Array(10, 1),
                            indices = TestUtil.MockIntArray(10, 2),
                            uvs = TestUtil.MockFloat2Array(10, 3),
                       }
                    }
                },
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