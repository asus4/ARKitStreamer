using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


namespace ARKitStream
{
    public static class TestUtil
    {
        public static Matrix4x4 MockMatrix(int n)
        {
            return new Matrix4x4(
                new Vector4(n + 0, n + 1, n + 2, n + 3),
                new Vector4(n + 4, n + 5, n + 6, n + 7),
                new Vector4(n + 8, n + 9, n + 10, n + 11),
                new Vector4(n + 12, n + 13, n + 14, n + 15)
            );
        }

        public static Pose MockPose(int n)
        {
            return new Pose()
            {
                position = new Vector3(n + 0, n + 1, n + 2),
                rotation = new Quaternion(n + 3, n + 4, n + 5, n + 6),
            };
        }

        public static int[] MockIntArray(int n, int offset)
        {
            var arr = new int[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = offset + i;
            }
            return arr;
        }

        public static float2[] MockFloat2Array(int n, int offset)
        {
            var arr = new float2[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = new float2(offset + i, offset + i);
            }
            return arr;
        }

        public static float3[] MockFloat3Array(int n, int offset)
        {
            var arr = new float3[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = new float3(offset + i, offset + i, offset + i);
            }
            return arr;
        }
    }
}