using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    [Serializable]
    public class ARKitRemotePacket
    {
        [Serializable]
        public struct CameraFrameEvent : IEquatable<CameraFrameEvent>
        {
            // public ARLightEstimationData lightEstimationl;
            public long timestampNs;
            public float4x4 projectionMatrix;
            public float4x4 displayMatrix;
            // public double? exposureDuration;
            // public float? exposureOffset;

            public bool Equals(CameraFrameEvent o)
            {
                return timestampNs.Equals(o.timestampNs)
                    && projectionMatrix.Equals(o.projectionMatrix)
                    && displayMatrix.Equals(o.displayMatrix);
            }

            public override string ToString()
            {
                return $"[time: {timestampNs}, projection: {projectionMatrix}, display: {displayMatrix}]";
            }

            // public static int DataSize => sizeof(long) + Marshal.SizeOf(typeof(Matrix4x4)) * 2;
        }

        [Serializable]
        public class FaceMesh
        {
            public uint2 id;
            public float3[] vertices;
            public float3[] normals;
            public int[] indices;
            public float2[] uvs;
        }

        [Serializable]
        public class FaceInfo
        {
            public XRFace[] added;
            public XRFace[] updated;
            public XRFace[] removed;
            public FaceMesh[] meshes;
        }

        public CameraFrameEvent cameraFrame;
        public FaceInfo face;


        static readonly BinaryFormatter formatter = new BinaryFormatter();
        static readonly MemoryStream stream = new MemoryStream();
        static byte[] buffer = new byte[0];

        public byte[] Serialize()
        {
            // BinaryFormatter is slow. shold make custom serialization?
            lock (stream)
            {
                stream.Position = 0;
                formatter.Serialize(stream, this);
                int length = (int)stream.Position;
                if (buffer.Length != length)
                {
                    buffer = new byte[length];
                }
                stream.Position = 0;
                stream.Read(buffer, 0, length);
            }
            return buffer;
        }

        public static ARKitRemotePacket Deserialize(byte[] data)
        {
            lock (stream)
            {
                stream.Position = 0;
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                return formatter.Deserialize(stream) as ARKitRemotePacket;
            }
        }
    }

}
