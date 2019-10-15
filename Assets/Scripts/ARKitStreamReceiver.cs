using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;

namespace ARKitStream
{
    [RequireComponent(typeof(NdiReceiver))]
    public class ARKitStreamReceiver : MonoBehaviour
    {
        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] ARHumanBodyManager humanBodyManager = null;

        NdiReceiver ndiReceiver = null;
        Vector2Int ndiSourceSize = Vector2Int.zero;

        CommandBuffer commandBuffer;
        Material bufferMaterial;

        RenderTexture cameraYTex;
        RenderTexture cameraCbCrTex;
        RenderTexture stencilTex;
        RenderTexture depthTex;

        void Start()
        {
            ndiReceiver = GetComponent<NdiReceiver>();

            commandBuffer = new CommandBuffer();
            commandBuffer.name ="ARKitStreamReceiver";

            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamReceiver"));
        }

        void OnDestroy()
        {
            commandBuffer.Dispose();
            commandBuffer = null;

            Release(cameraYTex);
            cameraYTex = null;

            Release(cameraCbCrTex);
            cameraCbCrTex = null;

            Release(stencilTex);
            stencilTex = null;

            Release(depthTex);
            depthTex = null;
        }

        void Update()
        {
            var rt = ndiReceiver.receivedTexture;
            if (rt == null)
            {
                return;
            }
            if (ndiSourceSize.x != rt.width || ndiSourceSize.y != rt.height)
            {
                InitTexture(rt);
                ndiSourceSize = new Vector2Int(rt.width, rt.height);
            }

            Debug.LogFormat("recevie rt: {0},{1}", rt.width, rt.height);

            commandBuffer.Clear();
            commandBuffer.Blit(rt, cameraYTex, bufferMaterial, 0);
            commandBuffer.Blit(rt, cameraCbCrTex, bufferMaterial, 1);
            commandBuffer.Blit(rt, stencilTex, bufferMaterial, 2);
            commandBuffer.Blit(rt, depthTex, bufferMaterial, 3);

            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        void OnGUI()
        {
            if (ndiSourceSize == Vector2Int.zero)
            {
                // Wait for connect
                return;
            }
            var w = Screen.width / 2;
            var h = Screen.height / 2;

            GUI.DrawTexture(new Rect(0, 0, w, h), cameraYTex);
            GUI.DrawTexture(new Rect(w, 0, w, h), cameraCbCrTex);
            GUI.DrawTexture(new Rect(0, h, w, h), stencilTex);
            GUI.DrawTexture(new Rect(w, h, w, h), depthTex);
        }

        void Release(RenderTexture tex)
        {
            if (tex == null)
            {
                return;
            }
            tex.Release();
            Destroy(tex);
        }

        void InitTexture(Texture source)
        {
            int width = source.width;
            int height = source.height / 2;

            cameraYTex = new RenderTexture(width, height, 0, RenderTextureFormat.R8);
            cameraCbCrTex = new RenderTexture(width, height, 0, RenderTextureFormat.RG16);
            stencilTex = new RenderTexture(width, height, 0, RenderTextureFormat.R8);
            depthTex = new RenderTexture(width, height, 0, RenderTextureFormat.RHalf);
        }

    }
}
