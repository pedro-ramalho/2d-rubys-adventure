using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(OptionsHandler))]
public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = SceneNames.Level0;

    [Header("Background Pan")]
    [SerializeField] private float backgroundPanSpeed = 0.15f;
    [SerializeField] private float backgroundPanAmplitudeX = 40f;
    [SerializeField] private float backgroundPanAmplitudeY = 20f;
    [SerializeField] private float backgroundScale = 1.15f;

    [Header("Title Bob")]
    [SerializeField] private float titleBobSpeed = 1.5f;
    [SerializeField] private float titleBobAmplitude = 12f;

    [Header("Click Sound")]
    [SerializeField] private AudioSource clickAudioSource;
    [SerializeField] private AudioClip clickClip;

    [Header("Start Game")]
    [SerializeField] private float startGameFadeDuration = 3f;

    private VisualElement background;
    private Label title;
    private VisualElement buttonContainer;
    private Button startButton;
    private Button continueButton;
    private Button optionsButton;
    private Button quitButton;
    private OptionsHandler optionsHandler;
    private bool isStartingGame;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        optionsHandler = GetComponent<OptionsHandler>();

        background = root.Q<VisualElement>("Background");
        title = root.Q<Label>("Title");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");

        if (background != null)
            background.style.scale = new Scale(new Vector3(backgroundScale, backgroundScale, 1f));

        startButton = root.Q<Button>("StartButton");
        continueButton = root.Q<Button>("ContinueButton");
        optionsButton = root.Q<Button>("OptionsButton");
        quitButton = root.Q<Button>("QuitButton");

        startButton.clicked += PlayClick;
        optionsButton.clicked += PlayClick;
        quitButton.clicked += PlayClick;
        continueButton.clicked += PlayClick;

        startButton.clicked += StartGame;
        optionsButton.clicked += optionsHandler.Open;
        quitButton.clicked += QuitGame;

        optionsHandler.Opened += () => buttonContainer.style.display = DisplayStyle.None;
        optionsHandler.Closed += () => buttonContainer.style.display = DisplayStyle.Flex;

        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
        {
            continueButton.SetEnabled(true);
            continueButton.clicked += ContinueGame;
        }
        else
        {
            continueButton.SetEnabled(false);
        }

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveDeleted += OnSaveDeleted;
    }

    void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveDeleted -= OnSaveDeleted;
    }

    void OnSaveDeleted()
    {
        if (continueButton == null) 
            return;
        
        continueButton.SetEnabled(false);
        continueButton.clicked -= ContinueGame;
    }

    public void PlayClick()
    {
        if (clickAudioSource != null && clickClip != null)
            clickAudioSource.PlayOneShot(clickClip);
    }

    void ContinueGame()
    {
        if (isStartingGame) 
            return;
        
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSave) 
            return;

        BeginSceneFadeAndLoad(SaveManager.Instance.Current.sceneName, writeSave: false);
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
        if (isStartingGame) 
            return;
        
        if (SaveManager.Instance != null) 
            SaveManager.Instance.DeleteSave();
        
        BeginSceneFadeAndLoad(firstLevelSceneName, writeSave: true);
    }

    void BeginSceneFadeAndLoad(string sceneName, bool writeSave)
    {
        isStartingGame = true;

        startButton.SetEnabled(false);
        continueButton.SetEnabled(false);
        optionsButton.SetEnabled(false);
        quitButton.SetEnabled(false);

        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutAndStop(startGameFadeDuration);

        StartCoroutine(FadeOutAndLoad(sceneName, writeSave));
    }

    IEnumerator FadeOutAndLoad(string sceneName, bool writeSave)
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement overlay = root.Q<VisualElement>("FadeOverlay");

        float elapsed = 0f;
        while (elapsed < startGameFadeDuration)
        {
            elapsed += Time.deltaTime;
            if (overlay != null)
                overlay.style.opacity = Mathf.Lerp(0f, 1f, elapsed / startGameFadeDuration);
            
            yield return null;
        }

        if (overlay != null) 
            overlay.style.opacity = 1f;

        if (writeSave && sceneName != SceneNames.MainMenu && SaveManager.Instance != null)
            SaveManager.Instance.WriteSave(sceneName);

        SceneManager.LoadScene(sceneName);
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
