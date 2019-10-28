using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.Mathematics;
using NUnit.Framework;

using XRFaceA = ARKitStream.Internal.XRFace;
using XRFaceB = UnityEngine.XR.ARSubsystems.XRFace;

namespace ARKitStream.Internal
{
    public class XRFaceTest
    {
        [Test]
        public static void ImplicitTest()
        {
            XRFaceA a = new XRFaceA()
            {
                trackableId = new TrackableId(ulong.MaxValue - 1000, ulong.MaxValue / 4),
                pose = TestUtil.MockPose(3),
                trackingState = TrackingState.Tracking,
                nativePtr = new IntPtr(8),
                leftEyePose = TestUtil.MockPose(9),
                rightEyePose = TestUtil.MockPose(17),
                fixationPoint = new float3(25, 26, 27)
            };

            XRFaceB b = a;



            Assert.AreEqual(a.trackableId.subId1, b.trackableId.subId1);
            Assert.AreEqual(a.trackableId.subId2, b.trackableId.subId2);

            XRFaceA c = b;

            Assert.AreEqual(a, c);

            Debug.Log($"A: {a}");
            Debug.Log($"C: {c}");

        }
    }
}

