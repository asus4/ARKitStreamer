using System;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    public class TextureDescriptorTest
    {
        [Test]
        public static void ImplicitTest()
        {
            TextureDescriptor d1 = new TextureDescriptor()
            {
                nativeTexture = new IntPtr(99),
                width = 10,
                height = 20,
                mipmapCount = 1,
                format = TextureFormat.R8,
                propertyNameId = 30
            };

            XRTextureDescriptor xr = d1;

            Debug.Log(xr);

            TextureDescriptor d2 = xr;

            Assert.AreEqual(d1, d2);
        }
    }
}
