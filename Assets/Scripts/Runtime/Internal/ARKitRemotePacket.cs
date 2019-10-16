using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using Unity.Mathematics;

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

        public CameraFrameEvent cameraFrame;

        static readonly BinaryFormatter formatter = new BinaryFormatter();
        static readonly MemoryStream stream = new MemoryStream();
        // static readonly byte[] buffer = new byte[CameraFrameEvent.DataSize];

        public byte[] Serialize()
        {
            lock (stream)
            {
                stream.Position = 0;
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
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
