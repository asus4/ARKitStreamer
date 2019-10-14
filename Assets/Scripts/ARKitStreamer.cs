using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARKitStreamer : MonoBehaviour
{
    [SerializeField] ARHumanBodyManager humanBodyManager = null;
    [SerializeField] Material material = null;

    static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");

    void Update()
    {
        var subsystem = humanBodyManager.subsystem;
        if (subsystem == null)
        {
            return;
        }

        var stencil = humanBodyManager.humanStencilTexture;
        material.SetTexture(_textureStencil, stencil);
    }
}
