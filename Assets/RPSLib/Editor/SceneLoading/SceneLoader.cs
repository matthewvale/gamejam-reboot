/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RPSEditor
{
    public class SceneLoader : EditorWindow
    {
        private const string _sceneSOPathKey = "SceneLoaderWindow.SceneSOPath";
        private SceneSO _sceneContainer;


        [MenuItem("RPSTools/Scene Loader", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<SceneLoader>("Scene Loader");
        }

        private void OnEnable()
        {
            string savedPath = EditorPrefs.GetString(_sceneSOPathKey, string.Empty);
            if (!string.IsNullOrEmpty(savedPath))
            {
                _sceneContainer = AssetDatabase.LoadAssetAtPath<SceneSO>(savedPath);
            }
        }

        private void OnDisable()
        {
            if (_sceneContainer != null)
            {
                string path = AssetDatabase.GetAssetPath(_sceneContainer);
                EditorPrefs.SetString(_sceneSOPathKey, path);
            }
            else
            {
                EditorPrefs.DeleteKey(_sceneSOPathKey);
            }
        }

        private void OnGUI()
        {
            var _newSceneContainer = (SceneSO)EditorGUILayout.ObjectField("Scene List", _sceneContainer, typeof(SceneSO), false);
            if (_newSceneContainer != _sceneContainer)
            {
                _sceneContainer = _newSceneContainer;
                OnDisable(); // Save immediately
            }

            if (_sceneContainer == null)
            {
                return;
            }

            foreach (var scene in _sceneContainer.Scenes)
            {
                if (scene == null)
                {
                    Debug.LogError("SceneSO contains a null SceneAsset reference. Check the list for empties.");
                    continue;
                }

                string scenePath = AssetDatabase.GetAssetPath(scene);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button($"{sceneName}"))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

    }

}