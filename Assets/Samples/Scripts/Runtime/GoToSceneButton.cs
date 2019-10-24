using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARKitStream
{
    [RequireComponent(typeof(Button))]
    public class GoToSceneButton : MonoBehaviour
    {
        public string sceneName = "";
        public LoadSceneMode mode = LoadSceneMode.Single;

        void OnEnable()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        void OnDisable()
        {
            var button = GetComponent<Button>();
            button.onClick.RemoveListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }
}