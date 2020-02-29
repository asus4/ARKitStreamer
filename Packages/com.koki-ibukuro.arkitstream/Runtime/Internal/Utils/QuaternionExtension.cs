using UnityEngine;
using Unity.Mathematics;

namespace ARKitStream.Internal
{
    public static class QuaternionExtension
    {
        public static float4 ToFloat4(this Quaternion q)
        {
            return new float4(q.x, q.y, q.z, q.w);
        }
    }
}