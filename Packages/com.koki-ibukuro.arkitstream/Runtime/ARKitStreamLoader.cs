using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#endif

using ARKitStream.Internal;

namespace ARKitStream
{

#if UNITY_EDITOR
    [XRSupportedBuildTarget(BuildTargetGroup.Standalone, new BuildTarget[] { BuildTarget.StandaloneOSX, BuildTarget.StandaloneWindows64 })]
#endif
    public class ARKitStreamLoader : XRLoaderHelper
    {
        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRCameraSubsystemDescriptor> s_CameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
        // static List<XRDepthSubsystemDescriptor> s_DepthSubsystemDescriptors = new List<XRDepthSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        // static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
        // static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
        static List<XRHumanBodySubsystemDescriptor> s_HumanBodySubsystemDescriptors = new List<XRHumanBodySubsystemDescriptor>();
        // static List<XREnvironmentProbeSubsystemDescriptor> s_EnvironmentProbeSubsystemDescriptors = new List<XREnvironmentProbeSubsystemDescriptor>();
        // static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        // static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
        // static List<XRObjectTrackingSubsystemDescriptor> s_ObjectTrackingSubsystemDescriptors = new List<XRObjectTrackingSubsystemDescriptor>();
        static List<XRFaceSubsystemDescriptor> s_FaceSubsystemDescriptors = new List<XRFaceSubsystemDescriptor>();
        static List<XROcclusionSubsystemDescriptor> s_OcclusionSubsystemDescriptors = new List<XROcclusionSubsystemDescriptor>();
        // static List<XRParticipantSubsystemDescriptor> s_ParticipantSubsystemDescriptors = new List<XRParticipantSubsystemDescriptor>();
        // static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptors = new List<XRMeshSubsystemDescriptor>();


        public override bool Initialize()
        {
            // if (!Application.isPlaying) return false;
#if UNITY_EDITOR
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, ARKitSessionRemoteSubsystem.ID);
            CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraSubsystemDescriptors, ARKitCameraRemoteSubsystem.ID);
            // CreateSubsystem<XRDepthSubsystemDescriptor, XRDepthSubsystem>(s_DepthSubsystemDescriptors, "ARKit-Depth");
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, ARKitXRPlaneRemoteSubsystem.ID);
            // CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, "ARKit-Anchor");
            // CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, "ARKit-Raycast");
            CreateSubsystem<XRHumanBodySubsystemDescriptor, XRHumanBodySubsystem>(s_HumanBodySubsystemDescriptors, ARKitHumanBodyRemoteSubsystem.ID);
            // CreateSubsystem<XREnvironmentProbeSubsystemDescriptor, XREnvironmentProbeSubsystem>(s_EnvironmentProbeSubsystemDescriptors, "ARKit-EnvironmentProbe");
            // CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "ARKit-Input");

            // Optional subsystems that might not have been registered, based on the iOS version.
            // CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(s_ImageTrackingSubsystemDescriptors, "ARKit-ImageTracking");
            // CreateSubsystem<XRObjectTrackingSubsystemDescriptor, XRObjectTrackingSubsystem>(s_ObjectTrackingSubsystemDescriptors, "ARKit-ObjectTracking");
            CreateSubsystem<XRFaceSubsystemDescriptor, XRFaceSubsystem>(s_FaceSubsystemDescriptors, ARKitFaceRemoteSubsystem.ID);
            CreateSubsystem<XROcclusionSubsystemDescriptor, XROcclusionSubsystem>(s_OcclusionSubsystemDescriptors, ARKitOcclusionRemoteSubsystem.ID);
            // CreateSubsystem<XRParticipantSubsystemDescriptor, XRParticipantSubsystem>(s_ParticipantSubsystemDescriptors, "ARKit-Participant");
            // CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptors, "ARKit-Meshing");
#endif

            Debug.Log("ARKitStreamLoader Initialize");

            var sessionSubsystem = GetLoadedSubsystem<XRSessionSubsystem>();
            if (sessionSubsystem == null)
            {
                Debug.LogError("Failed to load session subsystem.");
            }
            return sessionSubsystem != null;
        }

        public override bool Start()
        {
            return true;
        }

        public override bool Stop()
        {
            return true;
        }

        public override bool Deinitialize()
        {
            if (!Application.isPlaying)
            {
                // return base.Deinitialize();
            };

#if UNITY_EDITOR
            DestroySubsystem<XRCameraSubsystem>();
            // DestroySubsystem<XRDepthSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            // DestroySubsystem<XRAnchorSubsystem>();
            // DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRHumanBodySubsystem>();
            // DestroySubsystem<XREnvironmentProbeSubsystem>();
            // DestroySubsystem<XRInputSubsystem>();
            // DestroySubsystem<XRImageTrackingSubsystem>();
            // DestroySubsystem<XRObjectTrackingSubsystem>();
            DestroySubsystem<XRFaceSubsystem>();
            DestroySubsystem<XROcclusionSubsystem>();
            // DestroySubsystem<XRParticipantSubsystem>();
            // DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
#endif

            Debug.Log("ARKitStreamLoader Deinitialize");
            return base.Deinitialize();
        }
    }
}
