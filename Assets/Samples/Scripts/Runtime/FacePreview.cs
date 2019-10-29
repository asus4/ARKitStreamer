using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARKitStream
{
    public class FacePreview : MonoBehaviour
    {
        [SerializeField] ARFaceManager faceManager = null;

        void OnEnable()
        {
            faceManager.facesChanged += OnFaceChanged;
        }

        void OnDisable()
        {
            faceManager.facesChanged -= OnFaceChanged;
        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            // Debug.Log($"Face Changed: {args}");
        }
    }
}
