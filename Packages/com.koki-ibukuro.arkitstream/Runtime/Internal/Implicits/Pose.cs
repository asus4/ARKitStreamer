using System;
using UnityEngine;
using Unity.Mathematics;

namespace ARKitStream.Internal
{
    [Serializable]
    public struct Pose : IEquatable<Pose>
    {
        public float3 position;
        public float4 rotation;

        public bool Equals(Pose o)
        {
            return position.Equals(o.position)
                && rotation.Equals(o.rotation);
        }

        public override string ToString()
        {
            return string.Format("Pose:(p:{0}, r:{1})", position, rotation);
        }

        public static implicit operator Pose(UnityEngine.Pose p)
        {
            return new Pose()
            {
                position = p.position,
                rotation = new float4(p.rotation.x, p.rotation.y, p.rotation.z, p.rotation.w)
            };
        }

        public static implicit operator UnityEngine.Pose(Pose p)
        {
            return new UnityEngine.Pose(p.position, new quaternion(p.rotation));
        }

        public static Pose FromTransform(Transform t)
        {
            var q = t.localRotation;
            return new Pose()
            {
                position = t.localPosition,
                rotation = new float4(q.x, q.y, q.z, q.w),
            };
        }
    }
}
