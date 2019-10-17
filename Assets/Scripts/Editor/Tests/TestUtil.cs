using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}