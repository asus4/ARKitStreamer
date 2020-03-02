using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.Mathematics;


namespace ARKitStream.Internal
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundedPlane : IEquatable<BoundedPlane>
    {
        public TrackableId trackableId;
        public TrackableId subsumedById;
        public float2 center;
        public Pose pose;
        public float2 size;
        public PlaneAlignment alignment;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public PlaneClassification classification;

        public bool Equals(BoundedPlane o)
        {
            return trackableId.Equals(o.trackableId)
                && subsumedById.Equals(o.subsumedById)
                && center.Equals(o.center)
                && pose.Equals(o.pose)
                && size.Equals(o.size)
                && alignment.Equals(o.alignment)
                && trackingState.Equals(o.trackingState)
                && nativePtr.Equals(o.nativePtr)
                && classification.Equals(o.classification);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"[XRFace] id: {trackableId} ");
            sb.Append($"state: {trackingState} ");
            sb.Append($"size: {size} ");
            return sb.ToString();
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct BoundedPlaneUnion
        {
            [FieldOffset(0)] public BoundedPlane a;
            [FieldOffset(0)] public UnityEngine.XR.ARSubsystems.BoundedPlane b;
        }

        public static implicit operator UnityEngine.XR.ARSubsystems.BoundedPlane(BoundedPlane f)
        {
            var union = new BoundedPlaneUnion()
            {
                a = f,
            };
            return union.b;
        }

        public static implicit operator BoundedPlane(UnityEngine.XR.ARSubsystems.BoundedPlane f)
        {
            var union = new BoundedPlaneUnion()
            {
                b = f,
            };
            return union.a;
        }
    }
}
