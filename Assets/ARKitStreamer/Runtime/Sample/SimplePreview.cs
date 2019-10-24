using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SimplePreview : MonoBehaviour
{
    [SerializeField] ARHumanBodyManager humanBodyManager = null;
    [SerializeField] Material material = null;

    void Update()
    {
        var stencil = humanBodyManager.humanStencilTexture;
        var depth = humanBodyManager.humanDepthTexture;

        material.SetTexture("_textureStencil", stencil);
        material.SetTexture("_textureDepth", depth);
    }
}
