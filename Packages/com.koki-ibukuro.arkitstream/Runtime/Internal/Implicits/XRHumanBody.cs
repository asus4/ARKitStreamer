using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.Mathematics;

namespace ARKitStream.Internal
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XRHumanBody : IEquatable<XRHumanBody>
    {
        public TrackableId trackableId;
        public Pose pose;
        public float estimatedHeightScaleFactor;
        public TrackingState trackingState;
        public IntPtr nativePtr;

        public bool Equals(XRHumanBody other) => trackableId.Equals(other.trackableId);

        public override int GetHashCode() => trackableId.GetHashCode();

        [StructLayout(LayoutKind.Explicit)]
        private struct XRHumanBodyUnion
        {
            [FieldOffset(0)] public XRHumanBody a;
            [FieldOffset(0)] public UnityEngine.XR.ARSubsystems.XRHumanBody b;
        }

        public static implicit operator UnityEngine.XR.ARSubsystems.XRHumanBody(XRHumanBody o)
        {
            var union = new XRHumanBodyUnion()
            {
                a = o,
            };
            return union.b;
        }

        public static implicit operator XRHumanBody(UnityEngine.XR.ARSubsystems.XRHumanBody o)
        {
            var union = new XRHumanBodyUnion()
            {
                b = o,
            };
            return union.a;
        }
    }
}
