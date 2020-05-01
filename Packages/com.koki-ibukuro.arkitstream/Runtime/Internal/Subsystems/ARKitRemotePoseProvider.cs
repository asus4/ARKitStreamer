using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace ARKitStream.Internal
{
    [AddComponentMenu("")]
    public class ARKitRemotePoseProvider : BasePoseProvider
    {
        public Transform manualTarget = null;

        private void OnEnable()
        {
            Application.onBeforeRender += Update;
        }

        private void OnDisable()
        {
            Application.onBeforeRender -= Update;
        }

        private void Update()
        {
            if (manualTarget == null)
            {
                return;
            }

            UnityEngine.Pose pose;
            GetPoseFromProvider(out pose);
            manualTarget.localPosition = pose.position;
            manualTarget.localRotation = pose.rotation;
        }

        public override PoseDataFlags GetPoseFromProvider(out UnityEngine.Pose output)
        {
            var receiver = ARKitReceiver.Instance;
            if (receiver == null)
            {
                output = default(UnityEngine.Pose);
                return PoseDataFlags.NoData;
            }

            output = receiver.TrackedPose;
            return PoseDataFlags.Position | PoseDataFlags.Rotation;
        }
    }
}