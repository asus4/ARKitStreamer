using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARKitStream
{

    public class HumanBodyPreview : MonoBehaviour
    {
        [System.Serializable]
        public struct Joint
        {
            public Vector3 start;
            public Vector3 end;

            public Joint(Vector3 start, Vector3 end)
            {
                this.start = start;
                this.end = end;
            }
        }

        [SerializeField] ARHumanBodyManager humanBodyManager = null;

        [SerializeField] Mesh skeletonMesh = null;
        [SerializeField] Material material = null;
        List<Joint> joints = new List<Joint>();

        void Start()
        {
        }

        void OnEnable()
        {
            joints = new List<Joint>();
            humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        }
        void OnDisable()
        {
            joints.Clear();
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
        }

        void Update()
        {
            foreach (var joint in joints)
            {
                float length = Vector3.Distance(joint.start, joint.end);
                if (length < float.Epsilon)
                {
                    continue;
                }
                Debug.DrawLine(joint.start, joint.end, Color.green);
                var m = Matrix4x4.TRS(joint.start,
                                      Quaternion.LookRotation(joint.end - joint.start, Vector3.up),
                                      new Vector3(1, 1, length));
                Graphics.DrawMesh(skeletonMesh, m, material, gameObject.layer);
            }
        }

        void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
        {
            this.joints.Clear();
            foreach (var humanBody in eventArgs.updated)
            {
                AddHumanBody(humanBody);
            }
        }

        void AddHumanBody(ARHumanBody humanBody)
        {
            // Debug.Log(humanBody.ToString());
            Transform root = humanBody.transform;

            var joints = humanBody.joints;

            foreach (var joint in joints)
            {
                if (!joint.tracked || joint.parentIndex < 0)
                {
                    continue;
                }
                Vector3 start = root.TransformPoint(joint.anchorPose.position);
                Vector3 end = root.TransformPoint(joints[joint.parentIndex].anchorPose.position);
                this.joints.Add(new Joint(start, end));
            }
        }

    }
}
