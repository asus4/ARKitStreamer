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
            TrackableChangesModifier<UnityBoundedPlane> modifier = new TrackableChangesModifier<UnityBoundedPlane>();

            public override void Destroy()
            {
                modifier.Dispose();
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

                return modifier.Modify(added, updated, removed, allocator);
            }

            public override PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
                set { }
            }

            public override PlaneDetectionMode currentPlaneDetectionMode => PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
        }

    }
}
