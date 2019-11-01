using Unity.Collections;

namespace ARKitStream.Internal
{
    public static class NativeArrayExtension
    {
        public static byte[] ToRawBytes<T>(this NativeArray<T> arr) where T : struct
        {
            var slice = new NativeSlice<T>(arr).SliceConvert<byte>();
            var bytes = new byte[slice.Length];
            slice.CopyTo(bytes);
            return bytes;
        }

        public static void CopyFromRawBytes<T>(this NativeArray<T> arr, byte[] bytes) where T : struct
        {
            var byteArr = new NativeArray<byte>(bytes, Allocator.Temp);
            var slice = new NativeSlice<byte>(byteArr).SliceConvert<T>();

            UnityEngine.Debug.Assert(arr.Length == slice.Length);
            slice.CopyTo(arr);
        }
    }
}
