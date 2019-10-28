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
        public static void XRFaceImplicitTest()
        {
            XRFaceA a = new XRFaceA()
            {
                trackableId = new TrackableId(1, 2),
                pose = TestUtil.MockPose(3),
                trackingState = TrackingState.Tracking,
                nativePtr = new IntPtr(8),
                leftEyePose = TestUtil.MockPose(9),
                rightEyePose = TestUtil.MockPose(17),
                fixationPoint = new float3(25, 26, 27)
            };

            XRFaceB b = a;

            Debug.Log(b);

            XRFaceA c = b;

            Assert.AreEqual(a, c);
        }
    }
}

