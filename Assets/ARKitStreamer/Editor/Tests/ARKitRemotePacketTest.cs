using System;
using UnityEngine;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine.XR.ARSubsystems;


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
                    added = new ARKitStream.Internal.XRFace[]
                    {
                        new ARKitStream.Internal.XRFace()
                        {
                            trackableId = new TrackableId(5, 8),
                            pose = TestUtil.MockPose(2),
                            trackingState = TrackingState.Tracking,
                            nativePtr = new IntPtr(10),
                            leftEyePose = TestUtil.MockPose(11),
                            rightEyePose = TestUtil.MockPose(19),
                            fixationPoint = new float3(27, 28, 29),
                        }
                    },
                    updated = new ARKitStream.Internal.XRFace[]
                    {
                        new ARKitStream.Internal.XRFace()
                        {
                            trackableId = new TrackableId(0, 1),
                            pose = TestUtil.MockPose(2),
                            trackingState = TrackingState.Tracking,
                            nativePtr = new IntPtr(10),
                            leftEyePose = TestUtil.MockPose(11),
                            rightEyePose = TestUtil.MockPose(19),
                            fixationPoint = new float3(27, 28, 29),
                        }
                    },
                    removed = new TrackableId[]
                    {
                        new TrackableId(1, 2),
                        new TrackableId(3, 4),
                    },
                    meshes = new ARKitRemotePacket.FaceMesh[]
                    {
                        new ARKitRemotePacket.FaceMesh()
                        {
                            id= new TrackableId(5, 6),
                            // vertices = TestUtil.MockFloat3Array(10, 0),
                            // normals = TestUtil.MockFloat3Array(10, 1),
                            // indices = TestUtil.MockIntArray(10, 2),
                            // uvs = TestUtil.MockFloat2Array(10, 3),
                            vertices = new byte[] {7, 8, 9},
                            normals = new byte[] {10, 11, 12},
                            indices = new byte[] {13, 14, 15},
                            uvs = new byte[] {16, 17, 18},
                       }
                    }
                },
            };
            var data = a.Serialize();
            var b = ARKitRemotePacket.Deserialize(data);

            Assert.AreEqual(a.cameraFrame, b.cameraFrame);

            Debug.Log("binary size is : " + data.Length);


        }
    }
}