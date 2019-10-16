using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using WebSocketSharp;
using ARKitStream.Internal;

namespace ARKitStream
{
    [RequireComponent(typeof(NdiReceiver))]
    public class ARKitStreamReceiver : MonoBehaviour
    {
        [SerializeField] string ipAddress = "";
        [SerializeField] uint port = 8888;
        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] ARHumanBodyManager humanBodyManager = null;
        [SerializeField] bool isDrawGUI = false;
        [SerializeField] Material testMaterial;

        NdiReceiver ndiReceiver = null;
        Vector2Int ndiSourceSize = Vector2Int.zero;

        CommandBuffer commandBuffer;
        Material bufferMaterial;


        RenderTexture[] renderTextures;
        Texture2D[] texture2Ds;
        WebSocket client;

        object packetLock = new object();
        ARKitRemotePacket packet;

        void Start()
        {
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            ndiReceiver = GetComponent<NdiReceiver>();

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamReceiver";

            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamReceiver"));

            // 
            client = new WebSocket($"ws://{ipAddress}:{port}/arkit");
            client.OnOpen += (sender, e) =>
            {
                Debug.Log(e);
            };
            client.OnMessage += OnWebsocketMessage;
            client.ConnectAsync();
        }

        void OnDestroy()
        {
            if (commandBuffer != null)
            {
                commandBuffer.Dispose();
                commandBuffer = null;
            }
            if (renderTextures != null)
            {
                foreach (var rt in renderTextures)
                {
                    Release(rt);
                }
            }
            if (texture2Ds != null)
            {
                foreach (var tex in texture2Ds)
                {
                    Release(tex);
                }
            }
            if (client != null)
            {
                if (client.IsAlive)
                {
                    client.CloseAsync();
                }
            }
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

            // Decode Textures
            commandBuffer.Clear();
            for (int i = 0; i < renderTextures.Length; i++)
            {
                commandBuffer.Blit(rt, renderTextures[i], bufferMaterial, i);
            }

            Graphics.ExecuteCommandBuffer(commandBuffer);

            for (int i = 0; i < renderTextures.Length; i++)
            {
                Graphics.CopyTexture(renderTextures[i], texture2Ds[i]);
            }

            InvokeEvents();

            testMaterial.SetTexture("_textureStencil", texture2Ds[2]);
            testMaterial.SetTexture("_textureDepth", texture2Ds[3]);
        }

        void OnGUI()
        {
            if (!isDrawGUI)
            {
                return;
            }
            if (ndiSourceSize == Vector2Int.zero)
            {
                // Wait for connect
                return;
            }
            var w = Screen.width / 2;
            var h = Screen.height / 2;

            GUI.DrawTexture(new Rect(0, 0, w, h), texture2Ds[0]);
            GUI.DrawTexture(new Rect(w, 0, w, h), texture2Ds[1]);
            GUI.DrawTexture(new Rect(0, h, w, h), texture2Ds[2]);
            GUI.DrawTexture(new Rect(w, h, w, h), texture2Ds[3]);
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

        void Release(Texture tex)
        {
            if (tex == null)
            {
                return;
            }
            Destroy(tex);
        }

        void InitTexture(Texture source)
        {
            int width = source.width;
            int height = source.height / 2;

            var rformat = new RenderTextureFormat[] {
                RenderTextureFormat.R8, // Camera Y
                RenderTextureFormat.RG16, // Camera CbCr
                RenderTextureFormat.R8, // Stencil
                RenderTextureFormat.RHalf, // Depth
            };
            var tformat = new TextureFormat[] {
                TextureFormat.R8,
                TextureFormat.RG16,
                TextureFormat.R8,
                TextureFormat.RHalf,
            };

            renderTextures = new RenderTexture[rformat.Length];
            texture2Ds = new Texture2D[rformat.Length];

            for (int i = 0; i < rformat.Length; i++)
            {
                renderTextures[i] = new RenderTexture(width, height, 0, rformat[i]);
                texture2Ds[i] = new Texture2D(width, height, tformat[i], 1, false);
            }
        }

        void InvokeEvents()
        {
            // HACK: Invoke another class's event from refrection
            // https://stackoverflow.com/questions/198543/how-do-i-raise-an-event-via-reflection-in-net-c
            // cameraManager.frameReceived(args);

            var args = new ARCameraFrameEventArgs();
            args.textures = new List<Texture2D>() {
                texture2Ds[0],
                texture2Ds[1],
            };
            args.propertyNameIds = new List<int>() {
                Shader.PropertyToID("_textureY"),
                Shader.PropertyToID("_textureCbCr")
            };

            lock (packetLock)
            {
                if (packet != null)
                {
                    args.timestampNs = packet.cameraFrame.timestampNs;
                    args.displayMatrix = packet.cameraFrame.displayMatrix;
                    args.projectionMatrix = packet.cameraFrame.projectionMatrix;
                }
            }

            // Call private event field
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = cameraManager.GetType();
            var eventDelegate = (MulticastDelegate)type.GetField("frameReceived", flags).GetValue(cameraManager);
            Debug.AssertFormat(eventDelegate != null, "Check field name is collect.");

            var handlers = eventDelegate.GetInvocationList();
            foreach (var handler in handlers)
            {
                handler.Method.Invoke(handler.Target, new object[] { args });
            }

            // humanBodyManager.            
        }

        void OnWebsocketMessage(object sender, MessageEventArgs e)
        {
            try
            {
                lock (packetLock)
                {
                    packet = ARKitRemotePacket.Deserialize(e.RawData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

        }
    }
}
