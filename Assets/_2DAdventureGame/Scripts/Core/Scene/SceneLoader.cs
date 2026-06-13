using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "Game/Scene Loader")]
public class SceneLoader : ScriptableObject
{
    [SerializeField] private string sceneName;
    [SerializeField] private float preTransitionDelay = 4f;

    public void Load()
    {
        if (SceneTransitioner.Instance != null)
            SceneTransitioner.Instance.LoadSceneWithCrossfade(sceneName, preTransitionDelay);
    }
}
