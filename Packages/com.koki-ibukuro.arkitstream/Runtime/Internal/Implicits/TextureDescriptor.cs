using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    /// <summary>
    /// HACK: Need the unsafe struct cast
    /// since XRTextureDescriptor is private struct!
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureDescriptor : IEquatable<TextureDescriptor>
    {
        public IntPtr nativeTexture;
        public int width;
        public int height;
        public int mipmapCount;
        public TextureFormat format;
        public int propertyNameId;

        public TextureDescriptor(Texture2D tex, int propertyNameId)
        {
            nativeTexture = tex.GetNativeTexturePtr();
            width = tex.width;
            height = tex.height;
            mipmapCount = tex.mipmapCount;
            format = tex.format;
            this.propertyNameId = propertyNameId;
        }

        public bool Equals(TextureDescriptor other)
        {
            return nativeTexture.Equals(other.nativeTexture)
                && width.Equals(other.width)
                && height.Equals(other.height)
                && mipmapCount.Equals(other.mipmapCount)
                && format.Equals(other.format)
                && propertyNameId.Equals(other.propertyNameId);
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct TextureDescriptorUnion
        {
            [FieldOffset(0)] public TextureDescriptor a;
            [FieldOffset(0)] public XRTextureDescriptor b;
        }


        public static implicit operator XRTextureDescriptor(TextureDescriptor d)
        {
            var union = new TextureDescriptorUnion()
            {
                a = d,
            };
            return union.b;
        }

        public static implicit operator TextureDescriptor(XRTextureDescriptor d)
        {
            var union = new TextureDescriptorUnion()
            {
                b = d,
            };
            return union.a;
        }
    }
}
