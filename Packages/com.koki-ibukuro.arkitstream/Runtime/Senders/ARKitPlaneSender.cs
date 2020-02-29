using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;

using ARKitStream.Internal;
using BoundedPlane = ARKitStream.Internal.BoundedPlane;
using TrackableId = ARKitStream.Internal.TrackableId;

namespace ARKitStream
{
    public class ARKitPlaneSender : ARKitSubSender
    {
        [SerializeField] ARPlaneManager planeManager = null;

        ARKitRemotePacket.PlaneInfo planeInfo = null;

        protected override void Start()
        {
            base.Start();
            Debug.Log("Setup ARKitPlaneSender");
            planeManager.planesChanged += OnPlaneChanged;
        }

        protected override void OnDestroy()
        {
            planeManager.planesChanged -= OnPlaneChanged;
            base.OnDestroy();
        }

        void OnValidate()
        {
            if (planeManager == null)
            {
                planeManager = FindObjectOfType<ARPlaneManager>();
            }
        }

        void OnPlaneChanged(ARPlanesChangedEventArgs args)
        {
            var added = args.added.Select(plane => ToBoundedPlane(plane)).ToArray();
            var updated = args.updated.Select(plane => ToBoundedPlane(plane)).ToArray();
            var removed = args.removed.Select(plane => (TrackableId)plane.trackableId).ToArray();

            var meshes = new List<ARKitRemotePacket.PlaneMesh>();
            meshes.AddRange(args.added.Select(plane => ToMesh(plane)));
            meshes.AddRange(args.updated.Select(plane => ToMesh(plane)));

            planeInfo = new ARKitRemotePacket.PlaneInfo()
            {
                added = added,
                updated = updated,
                removed = removed,
                meshes = meshes.ToArray(),
            };
        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {
            packet.plane = planeInfo;
            planeInfo = null;
        }

        static BoundedPlane ToBoundedPlane(ARPlane plane)
        {
            return new BoundedPlane()
            {
                trackableId = plane.trackableId,
                subsumedById = plane.subsumedBy.trackableId,
                center = plane.centerInPlaneSpace,
                pose = Internal.Pose.FromTransform(plane.transform),
                size = plane.size,
                alignment = plane.alignment,
                trackingState = plane.trackingState,
                nativePtr = plane.nativePtr,
                classification = plane.classification,

            };
        }

        static ARKitRemotePacket.PlaneMesh ToMesh(ARPlane plane)
        {
            return new ARKitRemotePacket.PlaneMesh()
            {
                id = plane.trackableId,
                boundary = plane.boundary.ToRawBytes(),
            };
        }

        public static ARKitPlaneSender TryCreate(ARKitSender sender)
        {
            ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
            if (planeManager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitPlaneSender>();
            self.planeManager = planeManager;
            return self;
        }
    }
}
