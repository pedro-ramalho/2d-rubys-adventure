using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance { get; private set; }

    [SerializeField] private Animator transition;
    [SerializeField] private float preTransitionDelay = 1.5f;
    [SerializeField] private float transitionDuration = 1f;

    public bool IsTransitioning { get; private set; }

    void Awake() => Instance = this;

    public void LoadSceneWithCrossfade(string sceneName) => StartCoroutine(LoadScene(sceneName));

    IEnumerator LoadScene(string scene)
    {
        IsTransitioning = true;

        yield return new WaitForSeconds(preTransitionDelay);

        if (UIHandler.Instance != null)
        {
            UIHandler.Instance.HideDialogue();
            UIHandler.Instance.HideHUD();
        }
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(scene);
    }
}
