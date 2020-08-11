using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Unity.Mathematics;
using UnityEngine.XR.ARFoundation;
using ARKitStream.Internal;


namespace ARKitStream
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(ARKitSender))]
    public sealed class TrackedPoseSender : ARKitSubSender
    {
        [SerializeField] ARPoseDriver arPoseDriver = null;
        [SerializeField] TrackedPoseDriver trackedPoseDriver = null;

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {
            Transform t = null;
            bool useRelative = false;

            if (arPoseDriver != null)
            {
                t = arPoseDriver.transform;
                useRelative = true;
            }
            else if (trackedPoseDriver != null)
            {
                t = trackedPoseDriver.transform;
                useRelative = trackedPoseDriver.UseRelativeTransform;
            }

            if (t == null)
            {
                return;
            }

            var p = useRelative ? t.localPosition : t.position;
            var r = useRelative ? t.localRotation : t.rotation;
            packet.trackedPose = new Internal.Pose()
            {
                position = p,
                rotation = new float4(r.x, r.y, r.z, r.w),
            };
        }

        public static TrackedPoseSender TryCreate(ARKitSender sender)
        {
            TrackedPoseSender self = null;

            var trackedPoseDriver = FindObjectOfType<TrackedPoseDriver>();
            if (trackedPoseDriver != null)
            {
                self = sender.gameObject.AddComponent<TrackedPoseSender>();
                self.trackedPoseDriver = trackedPoseDriver;
            }

            var arPoseDriver = FindObjectOfType<ARPoseDriver>();
            if (arPoseDriver != null)
            {
                self = sender.gameObject.AddComponent<TrackedPoseSender>();
                self.arPoseDriver = arPoseDriver;
            }

            return self;
        }
    }
}
