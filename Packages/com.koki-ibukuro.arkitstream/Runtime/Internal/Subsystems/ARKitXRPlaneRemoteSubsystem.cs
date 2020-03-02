using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;


using UnityBoundedPlane = UnityEngine.XR.ARSubsystems.BoundedPlane;
using UnityTrackableId = UnityEngine.XR.ARSubsystems.TrackableId;

namespace ARKitStream.Internal
{
    [Preserve]
    public sealed class ARKitXRPlaneRemoteSubsystem : XRPlaneSubsystem
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_EDITOR
            const string id = "ARKit-Remote-Plane";
            var cinfo = new XRPlaneSubsystemDescriptor.Cinfo
            {
                id = id,
                subsystemImplementationType = typeof(ARKitXRPlaneRemoteSubsystem),
                supportsHorizontalPlaneDetection = true,
                supportsVerticalPlaneDetection = true,
                supportsArbitraryPlaneDetection = false,
                supportsBoundaryVertices = true,
                supportsClassification = true,
            };

            XRPlaneSubsystemDescriptor.Create(cinfo);
            Debug.LogFormat("Registered the {0} subsystem", id);
#endif
        }

        protected override Provider CreateProvider() => new ARKitRemoteProvider();

        class ARKitRemoteProvider : Provider
        {
            HashSet<UnityTrackableId> ids;

            public override void Destroy() { }
            public override void Start()
            {
                ids = new HashSet<UnityTrackableId>();
            }
            public override void Stop()
            {
                ids.Clear();
            }

            public override void GetBoundary(
                UnityTrackableId trackableId,
                Allocator allocator,
                ref NativeArray<Vector2> boundary)
            {
                var plane = ARKitReceiver.Instance?.Plane;
                byte[] rawBytes;
                if (plane == null || !plane.meshes.TryGetValue(trackableId, out rawBytes))
                {
                    CreateOrResizeNativeArrayIfNecessary(0, allocator, ref boundary);
                    return;
                }

                int size = rawBytes.Length / UnsafeUtility.SizeOf<Vector2>();
                CreateOrResizeNativeArrayIfNecessary(size, allocator, ref boundary);
                boundary.CopyFromRawBytes(rawBytes);
            }

            public override TrackableChanges<UnityBoundedPlane> GetChanges(
                UnityBoundedPlane defaultPlane,
                Allocator allocator)
            {
                var plane = ARKitReceiver.Instance?.Plane;
                if (plane == null)
                {
                    return new TrackableChanges<UnityBoundedPlane>();
                }

                var added = plane.added.Select(p => (UnityBoundedPlane)p).ToList();
                var updated = plane.updated.Select(p => (UnityBoundedPlane)p).ToList();
                var removed = plane.removed.Select(p => (UnityTrackableId)p).ToList();

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

                var nativeAdded = new NativeArray<UnityBoundedPlane>(added.ToArray(), Allocator.Temp);
                var nativeUpdated = new NativeArray<UnityBoundedPlane>(updated.ToArray(), Allocator.Temp);
                var nativeRemoved = new NativeArray<UnityTrackableId>(removed.ToArray(), Allocator.Temp);

                return TrackableChanges<UnityBoundedPlane>.CopyFrom(nativeAdded, nativeUpdated, nativeRemoved, allocator);
            }


        }

    }
}
