using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ARKitStream.Internal;

namespace ARKitStream
{
    public class ARKitFaceSender : ARKitSubSender
    {
        [SerializeField] ARFaceManager faceManager = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            faceManager.facesChanged += OnFaceChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
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

        }

        protected override void OnNdiTransformer(Material material)
        {

        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            Debug.Log($"Face Changed: {args}");
        }
    }
}
