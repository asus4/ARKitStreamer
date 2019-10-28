using System;
using System.Runtime.InteropServices;

using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;



namespace ARKitStream.Internal
{
    [Preserve]
    public class ARKitFaceRemoteSubsystem : XRFaceSubsystem
    {
        // this method is run on startup of the app to register this provider with XR Subsystem Manager
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_EDITOR
            const string id = "ARKit-Face-Remote";
            var descriptorParams = new FaceSubsystemParams
            {
                supportsFacePose = true,
                supportsFaceMeshVerticesAndIndices = true,
                supportsFaceMeshUVs = true,
                supportsEyeTracking = true,
                id = id,
                subsystemImplementationType = typeof(ARKitFaceRemoteSubsystem)
            };
            XRFaceSubsystemDescriptor.Create(descriptorParams);

            Debug.LogFormat("Registerd the {0} subsystem", id);
#endif // UNITY_EDITOR
        }

        protected override Provider CreateProvider()
        {
            return new ARKitRemoteProvider();
        }

        class ARKitRemoteProvider : Provider
        {
            public override TrackableChanges<UnityEngine.XR.ARSubsystems.XRFace> GetChanges(UnityEngine.XR.ARSubsystems.XRFace defaultFace, Allocator allocator)
            {
                return new TrackableChanges<UnityEngine.XR.ARSubsystems.XRFace>();
            }

            public override void GetFaceMesh(UnityEngine.XR.ARSubsystems.TrackableId faceId, Allocator allocator, ref XRFaceMesh faceMesh)
            {
            }
        }
    }
}
