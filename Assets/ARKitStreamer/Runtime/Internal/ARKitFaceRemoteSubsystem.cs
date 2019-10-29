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
                        added.Add(f);
                        ids.Add(f.trackableId);
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
                faceMesh.Resize(remoteMesh.vertices.Length / UnsafeUtility.SizeOf(typeof(Vector3)),
                                remoteMesh.indices.Length / sizeof(int) / 3, // count of triangles
                                attr, allocator);

                Debug.Log($"GetFaceMesh; {allocator}");
                // Debug.Log($"nativearray: vert:{faceMesh.vertices.Length} idx:{faceMesh.indices.Length}, uvs:{faceMesh.uvs.Length}");

                // faceMesh.vertices.CopyFrom(remoteMesh.vertices.Select(v => (Vector3)v).ToArray());
                // faceMesh.indices.CopyFrom(remoteMesh.indices.Select(i => i).ToArray());
                // faceMesh.uvs.CopyFrom(remoteMesh.uvs.Select(uv => (Vector2)uv).ToArray());
                faceMesh.vertices.CopyFromRawBytes(remoteMesh.vertices);
                faceMesh.indices.CopyFromRawBytes(remoteMesh.indices);
                faceMesh.uvs.CopyFromRawBytes(remoteMesh.uvs);


                // Show values
                var sb = new System.Text.StringBuilder();
                foreach (var v in faceMesh.vertices.Take(20))
                {
                    sb.AppendFormat("({0:0.0000},{1:0.0000},{2:0.0000}), ", v.x, v.y, v.z);
                }
                Debug.Log(sb.ToString());
            }
        }
    }
}
