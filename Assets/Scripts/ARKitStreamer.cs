using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;

public class ARKitStreamer : MonoBehaviour
{
    [SerializeField] ARCameraManager cameraManager = null;
    [SerializeField] ARHumanBodyManager humanBodyManager = null;
    [SerializeField] Material material = null;
    [SerializeField] RawImage debugImage = null;

    static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
    static readonly int _textureDepth = Shader.PropertyToID("_textureDepth");

    static readonly int k_DisplayTransformId = Shader.PropertyToID("_UnityDisplayTransform");

    Material bufferMaterial;
    RenderTexture renderTexture;
    NdiSender ndiSender;

    void Start()
    {
        bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamSender"));
        cameraManager.frameReceived += OnCameraFarameReceived;
    }

    void OnDestroy()
    {
        cameraManager.frameReceived -= OnCameraFarameReceived;

        Destroy(bufferMaterial);
        bufferMaterial = null;
    }

    void OnCameraFarameReceived(ARCameraFrameEventArgs args)
    {
        var subsystem = humanBodyManager.subsystem;
        if (subsystem == null)
        {
            return;
        }

        material.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
        material.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);


        if (renderTexture == null)
        {
            int width = args.textures.Max(t => t.width);
            int height = args.textures.Max(t => t.height);
            InitNDI(width, height);
        }

        var count = args.textures.Count;
        for (int i = 0; i < count; i++)
        {
            bufferMaterial.SetTexture(args.propertyNameIds[i], args.textures[i]);
        }
        if (args.displayMatrix.HasValue)
        {
            bufferMaterial.SetMatrix(k_DisplayTransformId, args.displayMatrix.Value);
        }
        Graphics.Blit(null, renderTexture, bufferMaterial);
    }

    void InitNDI(int width, int height)
    {
        Debug.LogFormat("Init NDI: {0} x {1}", width, height);

        renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        var name = string.Format("ARKit Stream {0:0000}", Random.Range(100, 9999));
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        ndiSender = go.AddComponent<NdiSender>();

        ndiSender.sourceTexture = renderTexture;
        ndiSender.alphaSupport = false;

        debugImage.texture = renderTexture;
    }

}
