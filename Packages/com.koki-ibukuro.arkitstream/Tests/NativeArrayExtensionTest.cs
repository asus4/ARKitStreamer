using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using NUnit.Framework;

namespace ARKitStream.Internal
{
    public static class NativeArrayExtensionTest
    {
        [Test]
        public static void ArrayEqual()
        {
            var arr1 = new NativeArray<int>(new int[] { -100, 100, 500 }, Allocator.Temp);
            var arr2 = new NativeArray<int>(new int[] { -100, 100, 500 }, Allocator.Temp);

            AreDeepEqual(arr1, arr2);
        }

        [Test]
        public static void IntToByte()
        {
            Arr2Byte(new int[] { -100, 100, 500 });
        }

        [Test]
        public static void FloatToByte()
        {
            Arr2Byte(new float[] { -100.0f, 1.5f, 9.9f, -32f });
        }

        [Test]
        public static void Vector3ToByte()
        {
            Arr2Byte(new Vector3[]
            {
                new Vector3(1, 2, 3),
                new Vector3(4, 5, 6),
                new Vector3(7, 8, 9),
            });
        }


        static void Arr2Byte<T>(T[] arr) where T : struct
        {
            var arr1 = new NativeArray<T>(arr, Allocator.Temp);
            var bytes = arr1.ToRawBytes();

            Assert.AreEqual(bytes.Length, arr.Length * UnsafeUtility.SizeOf<T>());

            var arr2 = new NativeArray<T>(arr.Length, Allocator.Temp);
            arr2.CopyFromRawBytes(bytes);

            AreDeepEqual(arr1, arr2);
        }

        static void AreDeepEqual<T>(NativeArray<T> a, NativeArray<T> b) where T : struct
        {
            Assert.AreEqual(a.Length, b.Length);
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i], b[i]);
            }
        }
    }
}
