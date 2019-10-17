using System;
using System.Runtime.InteropServices;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    public class CameraFrameTest
    {
        [Test]
        public static void ImplicitTest()
        {
            CameraFrame f1 = new CameraFrame()
            {
                timestampNs = 1,
                averageBrightness = 2f,
                averageColorTemperature = 3f,
                colorCorrection = Color.red,
                projectionMatrix = TestUtil.MockMatrix(4),
                displayMatrix = TestUtil.MockMatrix(20),
                trackingState = TrackingState.Tracking,
                nativePtr = new IntPtr(36),
                properties = XRCameraFrameProperties.DisplayMatrix,
                averageIntensityInLumens = 37f,
                exposureDuration = 38f,
                exposureOffset = 39f
            };

            XRCameraFrame xr = f1;

            Debug.Log(xr);

            CameraFrame f2 = xr;

            Assert.AreEqual(f1, f2);
        }
    }
}