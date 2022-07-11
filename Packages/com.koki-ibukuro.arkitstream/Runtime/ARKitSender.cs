using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using ARKitStream.Internal;

namespace ARKitStream
{
    [DisallowMultipleComponent]
    public sealed class ARKitSender : MonoBehaviour
    {
        [SerializeField] private ARCameraManager cameraManager = null;
        [SerializeField] private NdiResources resources = null;

        internal event Action<ARKitRemotePacket> PacketTransformer;
        internal event Action<Material> NdiTransformer;

        private Material bufferMaterial;
        private RenderTexture renderTexture;
        private NdiSender ndiSender;
        private CommandBuffer commandBuffer;

        private void Awake()
        {
            if (Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamSender";
            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamSender"));
            cameraManager.frameReceived += OnCameraFrameReceived;

            InitSubSenders();
        }

        private void OnDestroy()
        {
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnCameraFrameReceived;
            }

            if (bufferMaterial != null)
            {
                Destroy(bufferMaterial);
                bufferMaterial = null;
            }
            commandBuffer?.Dispose();
        }

        private void OnValidate()
        {
            if (cameraManager == null)
            {
                cameraManager = FindObjectOfType<ARCameraManager>();
            }
        }

        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            if (renderTexture == null)
            {
                int width = args.textures.Max(t => t.width);
                int height = args.textures.Max(t => t.height);
                InitNDI(width, height);
            }

            var packet = new ARKitRemotePacket()
            {
                cameraFrame = new ARKitRemotePacket.CameraFrameEvent()
                {
                    timestampNs = args.timestampNs.Value,
                    projectionMatrix = args.projectionMatrix.Value,
                    displayMatrix = args.displayMatrix.Value
                }
            };
            PacketTransformer?.Invoke(packet);
            ndiSender.metadata = packet.SerializeAsNdiMetadata();

            // Set texture
            var count = args.textures.Count;
            for (int i = 0; i < count; i++)
            {
                bufferMaterial.SetTexture(args.propertyNameIds[i], args.textures[i]);
            }
            NdiTransformer?.Invoke(bufferMaterial);

            commandBuffer.Blit(null, renderTexture, bufferMaterial);
            Graphics.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }

        private void InitNDI(int width, int height)
        {
            Debug.Log($"Init NDI width: {width} height: {height}");
            // test override size
            // width = width;
            // height = height * 2;

            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            var name = string.Format("ARKit Stream");
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);

            ndiSender = go.AddComponent<NdiSender>();
            ndiSender.SetResources(resources);
            ndiSender.captureMethod = CaptureMethod.Texture;
            ndiSender.keepAlpha = false;
            ndiSender.ndiName = "ARKit Stream";
            ndiSender.sourceTexture = renderTexture;
        }

        private void InitSubSenders()
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
