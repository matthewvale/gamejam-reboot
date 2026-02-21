/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPSCore
{
    public class StaticManagedUI : MonoBehaviour
    {
        public Canvas Canvas;
        [SerializeField] private List<string> ValidScenes;


        public virtual void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            ShowCanvas(scene);
        }

        public void ShowCanvas(Scene scene)
        {
            if (ValidScenes.Contains(scene.name))
            {
                Canvas.enabled = true;
            }
            else
            {
                Canvas.enabled = false;
            }
        }

    }
}