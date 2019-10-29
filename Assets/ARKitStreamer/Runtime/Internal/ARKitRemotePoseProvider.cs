using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

namespace ARKitStream.Internal
{
    public class ARKitRemotePoseProvider : BasePoseProvider
    {
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