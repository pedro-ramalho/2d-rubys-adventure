using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "Game/Scene Loader")]
public class SceneLoader : ScriptableObject
{
    public void LoadScene(string sceneName)
    {
        if (SceneTransitioner.Instance != null)
            SceneTransitioner.Instance.LoadSceneWithCrossfade(sceneName);
    }
}
