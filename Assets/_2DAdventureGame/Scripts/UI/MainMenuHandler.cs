using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level 0";

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("StartButton");
        Button continueButton = root.Q<Button>("ContinueButton");
        Button optionsButton = root.Q<Button>("OptionsButton");
        Button quitButton = root.Q<Button>("QuitButton");

        startButton.clicked += StartGame;
        quitButton.clicked += QuitGame;

        continueButton.SetEnabled(false);
        optionsButton.SetEnabled(false);
    }

    void StartGame() => SceneManager.LoadScene(firstLevelSceneName);

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
