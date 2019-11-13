using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using ARKitStream.Internal;

namespace ARKitStream
{
    public class BlendShapeInfo : MonoBehaviour
    {
        [SerializeField] ARFaceManager faceManager;
        [SerializeField] Text label;

        StringBuilder sb;

        void OnEnable()
        {
            sb = new StringBuilder();
            faceManager.facesChanged += OnFaceChanged;
        }

        void OnDisable()
        {
            sb.Clear();
            faceManager.facesChanged -= OnFaceChanged;
        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            var face = args.updated[0];

            var coeffients = GetCoefficients(face.trackableId, faceManager.subsystem);

            sb.Clear();
            sb.AppendLine("Coefficients");
            for (int i = 0; i < coeffients.Length; i++)
            {
                var c = coeffients[i];
                sb.Append(c.blendShapeLocation.ToString());
                sb.Append(" :\t");
                sb.Append(c.coefficient);
                sb.AppendLine();
            }

            label.text = sb.ToString();
        }

        static NativeArray<ARKitBlendShapeCoefficient> GetCoefficients(UnityEngine.XR.ARSubsystems.TrackableId id, XRFaceSubsystem subsystem)
        {
#if UNITY_EDITOR
            if (subsystem is ARKitFaceRemoteSubsystem)
            {
                return ((ARKitFaceRemoteSubsystem)subsystem).GetBlendShapeCoefficients(id, Allocator.Temp);
            }
#else
            if (subsystem is ARKitFaceSubsystem)
            {
                return ((ARKitFaceSubsystem)subsystem).GetBlendShapeCoefficients(id, Allocator.Temp);
            }
#endif
            return default(NativeArray<ARKitBlendShapeCoefficient>);
        }
    }
}
