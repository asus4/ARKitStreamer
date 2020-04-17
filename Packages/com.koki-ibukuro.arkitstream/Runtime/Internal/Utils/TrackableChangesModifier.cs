using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using UnityTrackableId = UnityEngine.XR.ARSubsystems.TrackableId;

namespace ARKitStream.Internal
{
    public class TrackableChangesModifier<T> : IDisposable where T : struct, ITrackable
    {
        HashSet<UnityTrackableId> ids;

        public TrackableChangesModifier()
        {
            ids = new HashSet<UnityTrackableId>();
        }

        public void Dispose()
        {
            ids.Clear();
            ids = null;
        }

        public TrackableChanges<T> Modify(List<T> added, List<T> updated, List<UnityTrackableId> removed, Allocator allocator)
        {
            // Check Added
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

            // Check Updated
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

            // Check Removed
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

            var nativeAdded = new NativeArray<T>(added.ToArray(), Allocator.Temp);
            var nativeUpdated = new NativeArray<T>(updated.ToArray(), Allocator.Temp);
            var nativeRemoved = new NativeArray<UnityTrackableId>(removed.ToArray(), Allocator.Temp);

            return TrackableChanges<T>.CopyFrom(nativeAdded, nativeUpdated, nativeRemoved, allocator);
        }
    }
}