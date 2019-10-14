using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARKitStreamer : MonoBehaviour
{
    [SerializeField] ARCameraManager cameraManager = null;
    [SerializeField] ARHumanBodyManager humanBodyManager = null;
    [SerializeField] Material material = null;

    static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
    static readonly int _textureDepth = Shader.PropertyToID("_textureDepth");

    void Start()
    {
        cameraManager.frameReceived += OnCameraFarameReceived;
    }

    void OnDestroy()
    {
        cameraManager.frameReceived -= OnCameraFarameReceived;
    }

    void Update()
    {
        var subsystem = humanBodyManager.subsystem;
        if (subsystem == null)
        {
            return;
        }

        material.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
        material.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);
    }

    void OnCameraFarameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log(args);
        var sb = new System.Text.StringBuilder();
        foreach (var tex in args.textures)
        {
            sb.AppendLine(tex.name);
        }
        Debug.Log(sb);
    }

}
