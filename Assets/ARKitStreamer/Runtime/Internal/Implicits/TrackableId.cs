using System;

namespace ARKitStream.Internal
{
    [Serializable]
    public class TrackableId
    {
        public ulong subId1;
        public ulong subId2;

        public TrackableId(ulong subId1, ulong subId2)
        {
            this.subId1 = subId1;
            this.subId2 = subId2;
        }

        public static implicit operator UnityEngine.XR.ARSubsystems.TrackableId(TrackableId id)
        {
            return new UnityEngine.XR.ARSubsystems.TrackableId(id.subId1, id.subId2);
        }

        public static implicit operator TrackableId(UnityEngine.XR.ARSubsystems.TrackableId id)
        {
            return new TrackableId(id.subId1, id.subId2);
        }
    }
}