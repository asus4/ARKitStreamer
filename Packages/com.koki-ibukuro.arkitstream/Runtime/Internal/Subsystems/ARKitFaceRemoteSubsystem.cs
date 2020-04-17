using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;
using UnityXRFace = UnityEngine.XR.ARSubsystems.XRFace;
using UnityTrackableId = UnityEngine.XR.ARSubsystems.TrackableId;


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

        public NativeArray<ARKitBlendShapeCoefficient> GetBlendShapeCoefficients(TrackableId faceId, Allocator allocator)
        {
            var face = ARKitReceiver.Instance?.Face;
            if (face == null)
            {
                Debug.LogWarning("face not found");
                return default(NativeArray<ARKitBlendShapeCoefficient>);
            }

            var mesh = face.meshes.FirstOrDefault(f => f.id.Equals(faceId));
            if (mesh == null)
            {
                Debug.LogWarning("face not found");
                return default(NativeArray<ARKitBlendShapeCoefficient>);
            }

            return NativeArrayExtension.FromRawBytes<ARKitBlendShapeCoefficient>(mesh.coefficients, allocator);
        }

        class ARKitRemoteProvider : Provider
        {
            HashSet<UnityTrackableId> ids = new HashSet<UnityTrackableId>();
            TrackableChangesModifier<UnityXRFace> modifier = new TrackableChangesModifier<UnityXRFace>();

            public override int supportedFaceCount => 1;
            public override int currentMaximumFaceCount => 1;

            public override TrackableChanges<UnityXRFace> GetChanges(UnityXRFace defaultFace, Allocator allocator)
            {

                var face = ARKitReceiver.Instance?.Face;
                if (face == null)
                {
                    return new TrackableChanges<UnityXRFace>();
                }
                // Debug.Log($"GetChanges: {face}");

                var added = face.added.Select(f => (UnityXRFace)f).ToList();
                var updated = face.updated.Select(f => (UnityXRFace)f).ToList();
                var removed = face.removed.Select(id => (UnityTrackableId)id).ToList();

                return modifier.Modify(added, updated, removed, allocator);
            }

            public override void GetFaceMesh(UnityTrackableId faceId, Allocator allocator, ref XRFaceMesh faceMesh)
            {

                var face = ARKitReceiver.Instance?.Face;
                var remoteMesh = face.meshes.FirstOrDefault(m => (UnityTrackableId)m.id == faceId);
                if (remoteMesh == null)
                {
                    Debug.LogWarning($"Mesh ID:{faceId} not found");
                    return;
                }


                XRFaceMesh.Attributes attr = XRFaceMesh.Attributes.UVs;
                faceMesh.Resize(remoteMesh.vertices.Length / UnsafeUtility.SizeOf<Vector3>(),
                                remoteMesh.indices.Length / sizeof(int) / 3, // count of triangles
                                attr, allocator);

                // Debug.Log($"GetFaceMesh; {allocator}");
                // Debug.Log($"nativearray: vert:{faceMesh.vertices.Length} idx:{faceMesh.indices.Length}, uvs:{faceMesh.uvs.Length}");

                faceMesh.vertices.CopyFromRawBytes(remoteMesh.vertices);
                faceMesh.indices.CopyFromRawBytes(remoteMesh.indices);
                faceMesh.uvs.CopyFromRawBytes(remoteMesh.uvs);
            }
        }
    }
}
