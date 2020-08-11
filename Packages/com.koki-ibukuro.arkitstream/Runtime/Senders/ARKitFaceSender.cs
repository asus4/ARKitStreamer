using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;

using ARKitStream.Internal;
using Pose = ARKitStream.Internal.Pose;
using XRFace = ARKitStream.Internal.XRFace;

namespace ARKitStream
{
    [AddComponentMenu("")]
    public class ARKitFaceSender : ARKitSubSender
    {
        [SerializeField] ARFaceManager faceManager = null;

        ARKitRemotePacket.FaceInfo info = null;

        protected override void Start()
        {
            base.Start();
            faceManager.facesChanged += OnFaceChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            faceManager.facesChanged -= OnFaceChanged;
        }

        void OnValidate()
        {
            if (faceManager == null)
            {
                faceManager = FindObjectOfType<ARFaceManager>();
            }
        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {
            packet.face = info;
            info = null;
        }

        protected override void OnNdiTransformer(Material material)
        {

        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            var subsystem = faceManager.subsystem as ARKitFaceSubsystem;
            info = new ARKitRemotePacket.FaceInfo();

            info.added = args.added.Select(face => ToARFace(face)).ToArray();
            info.updated = args.updated.Select(face => ToARFace(face)).ToArray();
            info.removed = args.removed.Select(face => (ARKitStream.Internal.TrackableId)face.trackableId).ToArray();

            var meshes = new List<ARKitRemotePacket.FaceMesh>();
            meshes.AddRange(args.added.Select(face => ToMesh(face, subsystem)));
            meshes.AddRange(args.updated.Select(face => ToMesh(face, subsystem)));
            // removed meshes are not needed
            // meshes.AddRange(args.removed.Select(face => ToMesh(face)));
            info.meshes = meshes.ToArray();
        }

        static XRFace ToARFace(ARFace face)
        {
            return new XRFace()
            {
                trackableId = face.trackableId,
                pose = Pose.FromTransform(face.transform),
                trackingState = face.trackingState,
                nativePtr = face.nativePtr,
                leftEyePose = Pose.FromTransform(face.leftEye),
                rightEyePose = Pose.FromTransform(face.rightEye),
                fixationPoint = face.fixationPoint.localPosition,
            };
        }

        static ARKitRemotePacket.FaceMesh ToMesh(ARFace face, ARKitFaceSubsystem subsystem)
        {
            var id = face.trackableId;
            return new ARKitRemotePacket.FaceMesh()
            {
                id = id,
                vertices = face.vertices.ToRawBytes(),
                normals = face.normals.ToRawBytes(),
                indices = face.indices.ToRawBytes(),
                uvs = face.uvs.ToRawBytes(),
                coefficients = subsystem.GetBlendShapeCoefficients(id, Allocator.Temp).ToRawBytes(),
            };
        }


        public static ARKitFaceSender TryCreate(ARKitSender sender)
        {
            ARFaceManager faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitFaceSender>();
            self.faceManager = faceManager;
            return self;
        }
    }
}
