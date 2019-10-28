using System.Linq;
using System.Collections.Generic;

using Unity.Jobs;
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

        class ARKitRemoteProvider : Provider
        {
            HashSet<UnityTrackableId> ids = new HashSet<UnityTrackableId>();

            public override int supportedFaceCount => 1;
            public override int maximumFaceCount => 1;

            public override TrackableChanges<UnityXRFace> GetChanges(UnityXRFace defaultFace, Allocator allocator)
            {

                var face = ARKitReceiver.Instance?.Face;
                if (face == null)
                {
                    return new TrackableChanges<UnityXRFace>();
                }
                // Debug.Log($"GetChanges: {face}");
                Debug.Log("GetChanges");

                var added = face.added.Select(f => (UnityXRFace)f).ToList();
                var updated = face.updated.Select(f => (UnityXRFace)f).ToList();
                var removed = face.removed.Select(id => (UnityTrackableId)id).ToList();

                foreach (var f in added.ToArray())
                {
                    if (ids.Contains(f.trackableId))
                    {
                        added.Remove(f);
                    }
                    else
                    {
                        ids.Add(f.trackableId);
                    }
                }

                foreach (var f in updated.ToArray())
                {
                    // Send as new
                    if (!ids.Contains(f.trackableId))
                    {
                        updated.Remove(f);
                        added.Append(f);
                    }
                }
                foreach (var id in removed.ToArray())
                {
                    // Send ad 
                    if (ids.Contains(id))
                    {
                        ids.Remove(id);
                    }
                    else
                    {
                        removed.Remove(id);
                    }
                }

                var nativeAdded = new NativeArray<UnityXRFace>(added.ToArray(), Allocator.Temp);
                var nativeUpdated = new NativeArray<UnityXRFace>(updated.ToArray(), Allocator.Temp);
                var nativeRemoved = new NativeArray<UnityTrackableId>(removed.ToArray(), Allocator.Temp);

                return TrackableChanges<UnityXRFace>.CopyFrom(nativeAdded, nativeUpdated, nativeRemoved, allocator);
            }

            public override void GetFaceMesh(UnityEngine.XR.ARSubsystems.TrackableId faceId, Allocator allocator, ref XRFaceMesh faceMesh)
            {
                Debug.Log("GetFaceMesh");

                var face = ARKitReceiver.Instance?.Face;
                var mesh = face.meshes.FirstOrDefault(m => (UnityEngine.XR.ARSubsystems.TrackableId)m.id == faceId);
                if (mesh == null)
                {
                    Debug.LogWarning($"Mesh ID:{faceId} not found");
                    return;
                }

                XRFaceMesh.Attributes attributes = XRFaceMesh.Attributes.Normals | XRFaceMesh.Attributes.UVs;

                faceMesh.Resize(mesh.vertices.Length, mesh.indices.Length, attributes, allocator);

                var vertices = faceMesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = mesh.vertices[i];
                }
                var normals = faceMesh.normals;
                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] = mesh.normals[i];
                }
                var indices = faceMesh.indices;
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = mesh.indices[i];
                }
                var uvs = faceMesh.uvs;
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = mesh.uvs[i];
                }

            }
        }
    }
}
