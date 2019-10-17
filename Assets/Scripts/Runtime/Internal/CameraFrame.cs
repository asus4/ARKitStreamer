using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraFrame : IEquatable<CameraFrame>
    {
        public long timestampNs;
        public float averageBrightness;
        public float averageColorTemperature;
        public Color colorCorrection;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 displayMatrix;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public XRCameraFrameProperties properties;
        public float averageIntensityInLumens;
        public double exposureDuration;
        public float exposureOffset;


        [StructLayout(LayoutKind.Explicit)]
        public struct CameraFrameUnion
        {
            [FieldOffset(0)] public CameraFrame a;
            [FieldOffset(0)] public XRCameraFrame b;
        }

        public bool Equals(CameraFrame o)
        {
            return timestampNs.Equals(o.timestampNs)
                && averageBrightness.Equals(o.averageBrightness)
                && averageColorTemperature.Equals(o.averageColorTemperature)
                && colorCorrection.Equals(o.colorCorrection)
                && projectionMatrix.Equals(o.projectionMatrix)
                && displayMatrix.Equals(o.displayMatrix)
                && trackingState.Equals(o.trackingState)
                && nativePtr.Equals(o.nativePtr)
                && properties.Equals(o.properties)
                && averageIntensityInLumens.Equals(o.averageIntensityInLumens)
                && exposureDuration.Equals(o.exposureDuration)
                && exposureOffset.Equals(o.exposureOffset);
        }

        public static implicit operator XRCameraFrame(CameraFrame f)
        {
            var union = new CameraFrameUnion()
            {
                a = f,
            };
            return union.b;
        }

        public static implicit operator CameraFrame(XRCameraFrame f)
        {
            var union = new CameraFrameUnion()
            {
                b = f,
            };
            return union.a;
        }
    }
}