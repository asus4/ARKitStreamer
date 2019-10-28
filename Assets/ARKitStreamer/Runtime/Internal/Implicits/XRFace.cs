using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.Mathematics;

namespace ARKitStream.Internal
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XRFace : IEquatable<XRFace>
    {
        public TrackableId trackableId;
        public Pose pose;
        public TrackingState trackingState;
        public IntPtr nativePtr;
        public Pose leftEyePose;
        public Pose rightEyePose;
        public float3 fixationPoint;


        public bool Equals(XRFace o)
        {
            return trackableId.Equals(o.trackableId)
                && pose.Equals(o.pose)
                && trackingState.Equals(o.trackingState)
                && nativePtr.Equals(o.nativePtr)
                && leftEyePose.Equals(o.leftEyePose)
                && rightEyePose.Equals(o.rightEyePose)
                && fixationPoint.Equals(o.fixationPoint);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"[XRFace] id: {trackableId} ");
            sb.Append($"pose: {pose} ");
            sb.Append($"state: {trackingState} ");
            sb.Append($"ptr: {nativePtr} ");
            sb.Append($"left: {leftEyePose} ");
            sb.Append($"right: {rightEyePose} ");
            sb.Append($"fixa: {fixationPoint} ");

            return sb.ToString();
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct XRFaceUnion
        {
            [FieldOffset(0)] public XRFace a;
            [FieldOffset(0)] public UnityEngine.XR.ARSubsystems.XRFace b;
        }

        public static implicit operator UnityEngine.XR.ARSubsystems.XRFace(XRFace f)
        {
            var union = new XRFaceUnion()
            {
                a = f,
            };
            return union.b;
        }

        public static implicit operator XRFace(UnityEngine.XR.ARSubsystems.XRFace f)
        {
            var union = new XRFaceUnion()
            {
                b = f,
            };
            return union.a;
        }
    }
}
