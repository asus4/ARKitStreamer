using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ARKitStream
{
    [CustomEditor(typeof(GoToSceneButton))]
    public class GoToSceneButtonEditor : Editor
    {
        GoToSceneButton _target;

        SerializedProperty sceneName;
        SerializedProperty mode;


        void OnEnable()
        {
            _target = target as GoToSceneButton;
            sceneName = serializedObject.FindProperty("sceneName");
            mode = serializedObject.FindProperty("mode");
        }

        public override void OnInspectorGUI()
        {
            // base.DrawDefaultInspector();
            // https://docs.unity3d.com/ScriptReference/SceneAsset.html?_ga=2.105493953.391722868.1571904603-1255573165.1486024520


            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_target.sceneName);

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            scene = EditorGUILayout.ObjectField("Scene", scene, typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck() && scene != null)
            {
                sceneName.stringValue = AssetDatabase.GetAssetPath(scene);
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.PropertyField(mode);
        }
    }
}
