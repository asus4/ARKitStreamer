using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.XR.ARSubsystems;
using NUnit.Framework;

using BoundedPlane = ARKitStream.Internal.BoundedPlane;
using UnityBoundedPlane = UnityEngine.XR.ARSubsystems.BoundedPlane;

namespace ARKitStream.Internal
{
    public class BoundedPlaneTest
    {
        // Start is called before the first frame update
        [Test]
        public static void ImplicitTest()
        {
            BoundedPlane p1 = new BoundedPlane()
            {
                trackableId = new TrackableId(0, 1),
                subsumedById = new TrackableId(2, 3),
                center = new float2(0.2f, 0.3f),
                pose = new Pose()
                {
                    position = new float3(0.1f, 0.2f, 0.3f),
                    rotation = Quaternion.EulerAngles(10, 20, 30).ToFloat4(),
                },
                size = new float2(1, 2),
                alignment = PlaneAlignment.Vertical,
                trackingState = TrackingState.Limited,
                nativePtr = new IntPtr(100),
                classification = PlaneClassification.Seat,
            };

            UnityBoundedPlane p2 = p1;
            Debug.Log(p2);

            BoundedPlane p3 = p2;

            Assert.AreEqual(p1, p3);
        }
    }

}
