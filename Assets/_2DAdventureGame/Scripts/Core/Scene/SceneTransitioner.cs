using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance { get; private set; }

    [SerializeField] private Animator transition;
    [SerializeField] private string startTrigger = "Start";
    [SerializeField] private string fadeOutClipName = "CrossfadeStart_Animation";
    [SerializeField] private float preTransitionDelay = 1.5f;
    private float fadeOutDuration = 1f;

    public bool IsTransitioning { get; private set; }

    void Awake()
    {
        Instance = this;
        fadeOutDuration = ResolveClipLength(fadeOutClipName);
        Debug.Log($"SceneTransitioner: fadeOutDuration = {fadeOutDuration}s");
    }

    float ResolveClipLength(string clipName)
    {
        if (transition == null || transition.runtimeAnimatorController == null)
            return 1f;

        foreach (var clip in transition.runtimeAnimatorController.animationClips)
            if (clip.name == clipName)
                return clip.length;

        Debug.LogWarning($"SceneTransitioner: clip '{clipName}' not found on the animator.");

        return 1f;
    }

    public void LoadSceneWithCrossfade(string sceneName) =>
        LoadSceneWithCrossfade(sceneName, preTransitionDelay);

    public void LoadSceneWithCrossfade(string sceneName, float preDelay) =>
        StartCoroutine(LoadScene(sceneName, preDelay));

    IEnumerator LoadScene(string scene, float preDelay)
    {
        IsTransitioning = true;

        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutAndStop(preDelay + fadeOutDuration);

        yield return new WaitForSeconds(preDelay);

        if (UIHandler.Instance != null)
        {
            UIHandler.Instance.HideDialogue();
            UIHandler.Instance.HideHUD();
        }
        transition.SetTrigger(startTrigger);

        yield return new WaitForSeconds(fadeOutDuration);

        if (scene != SceneNames.MainMenu && SaveManager.Instance != null)
            SaveManager.Instance.WriteSave(scene);

        SceneManager.LoadScene(scene);
    }
}
