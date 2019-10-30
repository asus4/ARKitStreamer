using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Mathematics;

namespace ARKitStream
{
    /// <summary>
    /// From 
    /// https://github.com/keijiro/Smrvfx
    /// </summary>
    public class FaceVFX : MonoBehaviour
    {
        [SerializeField] ARFaceManager faceManager = null;

        [SerializeField] RenderTexture positionTexture = null;
        [SerializeField] RenderTexture normalTexture = null;
        [SerializeField] RenderTexture velocityTexture = null;
        [SerializeField] ComputeShader compute = null;

        ComputeBuffer positionBuffer1;
        ComputeBuffer positionBuffer2;
        ComputeBuffer normalBuffer;

        RenderTexture positionTextureRW;
        RenderTexture normalTextureRW;
        RenderTexture velocityTextureRW;

        Matrix4x4 previousTransform = Matrix4x4.identity;

        void OnEnable()
        {
            faceManager.facesChanged += OnFaceChanged;
        }

        void OnDisable()
        {
            faceManager.facesChanged -= OnFaceChanged;

            positionBuffer1?.Dispose();
            positionBuffer2?.Dispose();
            normalBuffer?.Dispose();

            positionTextureRW?.Release();
            normalTextureRW?.Release();
            velocityTextureRW?.Release();
        }

        void OnValidate()
        {
            if (faceManager == null)
            {
                faceManager = FindObjectOfType<ARFaceManager>();
            }
        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            if (args.updated == null | args.updated.Count == 0)
            {
                return;
            }

            var face = faceManager.TryGetFace(args.updated[0].trackableId);
            BakeTexture(face);
        }

        void BakeTexture(UnityEngine.XR.ARFoundation.ARFace face)
        {
            int length = face.vertices.Length;

            if (positionBuffer1 == null || positionBuffer1.count != length * 3)
            {
                positionBuffer1?.Dispose();
                positionBuffer2?.Dispose();
                normalBuffer?.Dispose();

                positionBuffer1 = new ComputeBuffer(length * 3, sizeof(float));
                positionBuffer2 = new ComputeBuffer(length * 3, sizeof(float));
                normalBuffer = new ComputeBuffer(length * 3, sizeof(float));
            }

            if (positionTextureRW == null)
            {
                positionTextureRW = CreateRenderTextureRW(positionTexture);
                normalTextureRW = CreateRenderTextureRW(normalTexture);
                velocityTextureRW = CreateRenderTextureRW(velocityTexture);
            }


            compute.SetInt("VertexCount", length);
            compute.SetMatrix("Transform", face.transform.localToWorldMatrix);
            compute.SetMatrix("OldTransform", previousTransform);
            compute.SetFloat("FrameRate", 1 / Time.deltaTime);

            positionBuffer1.SetData(face.vertices);
            normalBuffer.SetData(face.GetComponent<MeshFilter>().sharedMesh.normals);

            compute.SetBuffer(0, "PositionBuffer", positionBuffer1);
            compute.SetBuffer(0, "OldPositionBuffer", positionBuffer2);
            compute.SetBuffer(0, "NormalBuffer", normalBuffer);

            compute.SetTexture(0, "PositionMap", positionTextureRW);
            compute.SetTexture(0, "VelocityMap", velocityTextureRW);
            compute.SetTexture(0, "NormalMap", normalTextureRW);

            compute.Dispatch(0, positionTexture.width / 8, positionTexture.height / 8, 1);

            Graphics.CopyTexture(positionTextureRW, positionTexture);
            Graphics.CopyTexture(velocityTextureRW, velocityTexture);
            Graphics.CopyTexture(normalTextureRW, normalTexture);

            previousTransform = face.transform.localToWorldMatrix;
        }

        static RenderTexture CreateRenderTextureRW(RenderTexture source)
        {
            var rt = new RenderTexture(source.width, source.height, 0, source.format);
            rt.enableRandomWrite = true;
            rt.Create();
            return rt;
        }

    }
}
