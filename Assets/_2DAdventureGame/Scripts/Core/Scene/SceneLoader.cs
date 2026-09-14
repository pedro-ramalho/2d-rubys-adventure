using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Scene
{
    [CreateAssetMenu(fileName = "SceneLoader", menuName = "Game/Scene Loader")]
    public class SceneLoader : ScriptableObject
    {
        [FormerlySerializedAs("sceneName")]
        [SerializeField] private string m_SceneName;
        
        [FormerlySerializedAs("preTransitionDelay")]
        [SerializeField] private float m_PreTransitionDelay = 4f;

        public void Load()
        {
            if (SceneTransitioner.Instance != null)
                SceneTransitioner.Instance.LoadSceneWithCrossfade(m_SceneName, m_PreTransitionDelay);
        }
    }
}
