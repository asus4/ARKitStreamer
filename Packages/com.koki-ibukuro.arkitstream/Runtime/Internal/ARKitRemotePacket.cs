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
            public TrackableId id;
            public byte[] vertices; // NativeArray<Vector3>
            public byte[] normals; // NativeArray<Vector3>
            public byte[] indices; // NativeArray<int>
            public byte[] uvs; // NativeArray<Vector2>
            public byte[] coefficients;

            public override string ToString()
            {
                return $"Mesh {id} verts: {vertices.Length} norms: {normals.Length} indices: {indices.Length} uvs: {uvs.Length}";
            }
        }

        [Serializable]
        public class FaceInfo
        {
            public XRFace[] added;
            public XRFace[] updated;
            public TrackableId[] removed;
            public FaceMesh[] meshes;

            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("FaceInfo");
                foreach (var f in added)
                {
                    sb.AppendLine($"ADD: {f}");
                }
                foreach (var f in updated)
                {
                    sb.AppendLine($"UPDATE: {f}");
                }
                foreach (var f in removed)
                {
                    sb.AppendLine($"REMOVE: {f}");
                }
                foreach (var m in meshes)
                {
                    sb.AppendLine($"MESHED: {m}");
                }
                return sb.ToString();
            }
        }

        public CameraFrameEvent cameraFrame;
        public FaceInfo face;
        public Pose trackedPose;


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
