using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.Mathematics;
using NUnit.Framework;

using XRHumanBodyA = ARKitStream.Internal.XRHumanBody;
using XRHumanBodyB = UnityEngine.XR.ARSubsystems.XRHumanBody;

namespace ARKitStream.Internal
{
    public class XRHumanBodyTest
    {
        [Test]
        public static void ImplicitTest()
        {
            XRHumanBodyA a = new XRHumanBodyA()
            {
                trackableId = new TrackableId(ulong.MaxValue - 1000, ulong.MaxValue / 4),
                pose = TestUtil.MockPose(3),
                estimatedHeightScaleFactor = 4,
                trackingState = TrackingState.Tracking,
                nativePtr = new IntPtr(8),
            };

            XRHumanBodyB b = a;

            Assert.AreEqual(a.trackableId.subId1, b.trackableId.subId1);
            Assert.AreEqual(a.trackableId.subId2, b.trackableId.subId2);

            XRHumanBodyA c = b;

            Assert.AreEqual(a, c);
        }
    }

}
