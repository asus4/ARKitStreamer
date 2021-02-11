using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using WebSocketSharp;
using WebSocketSharp.Server;
using ARKitStream.Internal;

namespace ARKitStream
{
    [RequireComponent(typeof(NdiSender))]
    public sealed class ARKitSender : MonoBehaviour
    {
        public sealed class ARKitService : WebSocketBehavior
        {
            public bool IsOpen { get; private set; }
            public ARKitService() { }

            protected override void OnOpen()
            {
                base.OnOpen();
                Debug.Log($"Socket opened");
                IsOpen = true;
            }

            protected override void OnClose(CloseEventArgs e)
            {
                base.OnClose(e);
                Debug.Log($"Socket closed: {e}");
                IsOpen = false;
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                base.OnMessage(e);
                Debug.Log(e);
            }

            public void ExternalSend(byte[] data)
            {
                Send(data);
            }

            public void ExternalSendAsync(byte[] data)
            {
                SendAsync(data, null);
            }

            public void ExternalClose()
            {
                if (!IsOpen) return;

                try
                {
                    Close();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
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

        void Awake()
        {
            if (Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }
        }

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

            InitSubSenders();

            ndiSender = GetComponent<NdiSender>();
        }

        void OnDestroy()
        {
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnCameraFarameReceived;
            }

            if (bufferMaterial != null)
            {
                Destroy(bufferMaterial);
                bufferMaterial = null;
            }

            service?.ExternalClose();
            server?.Stop();
        }

        void OnValidate()
        {
            if (cameraManager == null)
            {
                cameraManager = FindObjectOfType<ARCameraManager>();
            }

            NdiSender ndiSender;
            if (!TryGetComponent<NdiSender>(out ndiSender))
            {
                ndiSender = gameObject.AddComponent<NdiSender>();
            }
            ndiSender.captureMethod = CaptureMethod.Texture;
            ndiSender.enableAlpha = false;
            ndiSender.ndiName = "ARKit Stream";
        }

        void OnCameraFarameReceived(ARCameraFrameEventArgs args)
        {
            if (service != null && service.IsOpen)
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
                // service.ExternalSend(packet.Serialize());
                service.ExternalSendAsync(packet.Serialize());
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
            Debug.Log($"Init NDI withd: {width} height: {height}");
            // test override size
            // width = width;
            // height = height * 2;

            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            var name = string.Format("ARKit Stream");
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);

            ndiSender.sourceTexture = renderTexture;
        }

        void InitSubSenders()
        {
            TrackedPoseSender.TryCreate(this);
            ARKitFaceSender.TryCreate(this);
            ARKitOcclusionSender.TryCreate(this);
            ARKitPlaneSender.TryCreate(this);
            ARKitHumanBodySender.TryCreate(this);
            ARKitEnvironmentProbeSender.TryCreate(this);
        }

    }
}
