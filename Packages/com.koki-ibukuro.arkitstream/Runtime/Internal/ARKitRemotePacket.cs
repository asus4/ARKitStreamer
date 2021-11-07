using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ARKitStream.Internal
{
    [Serializable]
    public partial class ARKitRemotePacket
    {
        public CameraFrameEvent cameraFrame;
        public FaceInfo face;
        public Pose trackedPose;
        public PlaneInfo plane;
        public HumanBodyInfo humanBody;

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

        public string SerializeAsNdiMetadata()
        {
            byte[] data = Serialize();
            return "<![CDATA[" + Convert.ToBase64String(data) + "]] >";
        }

        public static ARKitRemotePacket DeserializeFromNdiMetadata(string xml)
        {
            // ref: https://github.com/keijiro/Rcam2 (unlicense license)
            string base64 = xml.Substring(9, xml.Length - 9 - 4);
            byte[] data = Convert.FromBase64String(base64);
            return Deserialize(data);
        }
    }

}
