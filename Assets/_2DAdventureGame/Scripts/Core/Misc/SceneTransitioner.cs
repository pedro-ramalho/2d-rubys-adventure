using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance { get; private set; }

    [SerializeField] private Animator transition;

    void Awake() => Instance = this;

    public void LoadSceneWithCrossfade(string sceneName) => StartCoroutine(LoadScene(sceneName));

    IEnumerator LoadScene(string scene)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(scene);
    }
}
