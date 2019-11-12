using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using WebSocketSharp;
using WebSocketSharp.Server;
using ARKitStream.Internal;

namespace ARKitStream
{
    public sealed class ARKitSender : MonoBehaviour
    {
        public class ARKitService : WebSocketBehavior
        {
            public ARKitService()
            {

            }
            protected override void OnMessage(MessageEventArgs e)
            {
                Debug.Log(e);
            }

            public void ExternalSend(byte[] data)
            {
                Send(data);
            }

            public void ExternalSend(string data)
            {
                Send(data);
            }
        }

        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] uint port = 8888;

        internal event Action<ARKitRemotePacket> PacketTransformer;
        internal event Action<Material> NdiTransformer;

        Material bufferMaterial;
        RenderTexture renderTexture;
        NdiSender ndiSender;
        CommandBuffer commandBuffer;
        WebSocketServer server;
        ARKitService service;

        void Start()
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamSender";
            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamSender"));
            cameraManager.frameReceived += OnCameraFarameReceived;

            server = new WebSocketServer((int)port);
            server.AddWebSocketService<ARKitService>("/arkit", (behaviour) =>
            {
                this.service = behaviour;
            });
            server.Start();

        }

        void OnDestroy()
        {
            cameraManager.frameReceived -= OnCameraFarameReceived;

            Destroy(bufferMaterial);
            bufferMaterial = null;

            server.Stop();
        }

        void OnCameraFarameReceived(ARCameraFrameEventArgs args)
        {            
            if (service != null)
            {
                var packet = new ARKitRemotePacket()
                {
                    cameraFrame = new ARKitRemotePacket.CameraFrameEvent()
                    {
                        timestampNs = args.timestampNs.Value,
                        projectionMatrix = args.projectionMatrix.Value,
                        displayMatrix = args.displayMatrix.Value
                    }
                };
                if (PacketTransformer != null)
                {
                    PacketTransformer(packet);
                }
                service.ExternalSend(packet.Serialize());
            }



            if (renderTexture == null)
            {
                int width = args.textures.Max(t => t.width);
                int height = args.textures.Max(t => t.height);
                InitNDI(width, height);
            }

            // Set texture
            var count = args.textures.Count;
            for (int i = 0; i < count; i++)
            {
                bufferMaterial.SetTexture(args.propertyNameIds[i], args.textures[i]);
            }

            if (NdiTransformer != null)
            {
                NdiTransformer(bufferMaterial);
            }

            commandBuffer.Blit(null, renderTexture, bufferMaterial);
            Graphics.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }

        void InitNDI(int width, int height)
        {
            Debug.Log("InitNDI");
            renderTexture = new RenderTexture(width, height * 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            var name = string.Format("ARKit Stream {0:0000}", UnityEngine.Random.Range(100, 9999));
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            ndiSender = go.AddComponent<NdiSender>();

            ndiSender.sourceTexture = renderTexture;
            ndiSender.alphaSupport = false;
        }

    }
}
