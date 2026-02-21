/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPSEditor
{
    [CreateAssetMenu(fileName = "SceneContainer", menuName = "RPS Tools/Editor/New Scene Container")]
    public class SceneSO : ScriptableObject
    {
        public List<SceneAsset> Scenes;
    }
}