using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Unity.Mathematics;
using ARKitStream.Internal;


namespace ARKitStream
{
    [RequireComponent(typeof(ARKitSender))]
    public sealed class TrackedPoseSender : ARKitSubSender
    {
        [SerializeField] TrackedPoseDriver driver = null;


        void OnValidate()
        {
            if (driver == null)
            {
                driver = FindObjectOfType<TrackedPoseDriver>();
            }

        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {
            if (driver == null)
            {
                return;
            }

            var t = driver.transform;
            var p = driver.UseRelativeTransform ? t.localPosition : t.position;
            var r = driver.UseRelativeTransform ? t.localRotation : t.rotation;
            packet.trackedPose = new Internal.Pose()
            {
                position = p,
                rotation = new float4(r.x, r.y, r.z, r.w),
            };
        }
    }
}
