using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARKitStream
{
    public class SimplePreview : MonoBehaviour
    {
        [SerializeField] AROcclusionManager occlusionManager = null;
        [SerializeField] Material material = null;

        void Update()
        {
            var stencil = occlusionManager.humanStencilTexture;
            var depth = occlusionManager.humanDepthTexture;

            material.SetTexture("_textureStencil", stencil);
            material.SetTexture("_textureDepth", depth);
        }
    }
}
