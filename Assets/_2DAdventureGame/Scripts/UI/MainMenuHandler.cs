using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(OptionsHandler))]
public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level 0";

    [Header("Background Pan")]
    [SerializeField] private float backgroundPanSpeed = 0.15f;
    [SerializeField] private float backgroundPanAmplitudeX = 40f;
    [SerializeField] private float backgroundPanAmplitudeY = 20f;
    [SerializeField] private float backgroundScale = 1.15f;

    [Header("Title Bob")]
    [SerializeField] private float titleBobSpeed = 1.5f;
    [SerializeField] private float titleBobAmplitude = 12f;

    private VisualElement background;
    private Label title;
    private VisualElement buttonContainer;
    private OptionsHandler optionsHandler;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        optionsHandler = GetComponent<OptionsHandler>();

        background = root.Q<VisualElement>("Background");
        title = root.Q<Label>("Title");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");

        if (background != null)
            background.style.scale = new Scale(new Vector3(backgroundScale, backgroundScale, 1f));

        Button startButton = root.Q<Button>("StartButton");
        Button continueButton = root.Q<Button>("ContinueButton");
        Button optionsButton = root.Q<Button>("OptionsButton");
        Button quitButton = root.Q<Button>("QuitButton");

        startButton.clicked += StartGame;
        optionsButton.clicked += optionsHandler.Open;
        quitButton.clicked += QuitGame;

        optionsHandler.Opened += () => buttonContainer.style.display = DisplayStyle.None;
        optionsHandler.Closed += () => buttonContainer.style.display = DisplayStyle.Flex;

        continueButton.SetEnabled(false);
    }

    void Update()
    {
        if (background != null)
        {
            float bx = Mathf.Sin(Time.time * backgroundPanSpeed) * backgroundPanAmplitudeX;
            float by = Mathf.Cos(Time.time * backgroundPanSpeed * 0.7f) * backgroundPanAmplitudeY;
            background.style.translate = new Translate(bx, by);
        }

        if (title != null)
        {
            float ty = Mathf.Sin(Time.time * titleBobSpeed) * titleBobAmplitude;
            title.style.translate = new Translate(0, ty);
        }
    }

    void StartGame()
    {
        if (SceneTransitioner.Instance != null)
            SceneTransitioner.Instance.LoadSceneWithCrossfade(firstLevelSceneName);
        else
            SceneManager.LoadScene(firstLevelSceneName);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
