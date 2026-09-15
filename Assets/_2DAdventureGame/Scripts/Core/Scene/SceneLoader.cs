using UnityEngine;

namespace AdventureGame.Core.Scene
{
    [CreateAssetMenu(fileName = "SceneLoader", menuName = "Game/Scene Loader")]
    public class SceneLoader : ScriptableObject
    {
        [SerializeField]
        private string m_SceneName;

        [SerializeField]
        private float m_PreTransitionDelay = 4f;

        public void Load()
        {
            if (SceneTransitioner.Instance != null)
                SceneTransitioner.Instance.LoadSceneWithCrossfade(
                    m_SceneName,
                    m_PreTransitionDelay
                );
        }
    }
}
