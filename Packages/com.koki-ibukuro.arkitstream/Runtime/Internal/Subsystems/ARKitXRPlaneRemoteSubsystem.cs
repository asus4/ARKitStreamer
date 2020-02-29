using System.Linq;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using UnityBoundedPlane = UnityEngine.XR.ARSubsystems.BoundedPlane;
using UnityTrackableId = UnityEngine.XR.ARSubsystems.TrackableId;

namespace ARKitStream.Internal
{
    [Preserve]
    public sealed class ARKitXRPlaneRemoteSubsystem : XRPlaneSubsystem
    {
        protected override Provider CreateProvider() => new ARKitRemoteProvider();

        class ARKitRemoteProvider : Provider
        {
            public override void Destroy() { }
            public override void Start() { }
            public override void Stop() { }
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

                var nativeAdded = new NativeArray<UnityBoundedPlane>(added.ToArray(), Allocator.Temp);
                var nativeUpdated = new NativeArray<UnityBoundedPlane>(updated.ToArray(), Allocator.Temp);
                var nativeRemoved = new NativeArray<UnityTrackableId>(removed.ToArray(), Allocator.Temp);

                return TrackableChanges<UnityBoundedPlane>.CopyFrom(nativeAdded, nativeUpdated, nativeRemoved, allocator);
            }
        }
    }
}
