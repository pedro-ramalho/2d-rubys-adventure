using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : PersistentSingleton<PauseManager>
{
    public static bool IsPaused { get; private set; }

    [SerializeField] private UIDocument pauseDocument;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip toggleClip;

    private PlayerInputActions inputActions;
    private VisualElement pauseRoot;
    private Label saveConfirmationLabel;
    private Coroutine saveConfirmationRoutine;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        inputActions = InputManager.Instance.Actions;
    }

    void OnEnable()
    {
        if (Instance != this) return;
        inputActions.Player.Pause.performed += OnPausePressed;
    }

    void OnDisable()
    {
        if (Instance != this) return;
        inputActions.Player.Pause.performed -= OnPausePressed;
    }

    void Start()
    {
        if (Instance != this) return;

        pauseRoot = pauseDocument.rootVisualElement.Q<VisualElement>("PauseRoot");
        saveConfirmationLabel = pauseRoot.Q<Label>("SaveConfirmationLabel");
        Button saveButton = pauseRoot.Q<Button>("SaveButton");
        Button returnButton = pauseRoot.Q<Button>("ReturnButton");

        saveButton.clicked += () =>
        {
            SaveManager.Instance?.WriteSave(SceneManager.GetActiveScene().name);
            ShowSaveFeedback();
        };

        returnButton.clicked += () =>
        {
            Resume();
            SceneTransitioner.Instance?.LoadSceneWithCrossfade(SceneNames.MainMenu);
        };

        SetVisible(false);
    }

    void ShowSaveFeedback()
    {
        if (saveConfirmationLabel == null)
            return;

        if (saveConfirmationRoutine != null)
            StopCoroutine(saveConfirmationRoutine);

        saveConfirmationRoutine = StartCoroutine(SaveFeedbackRoutine());
    }

    IEnumerator SaveFeedbackRoutine()
    {
        saveConfirmationLabel.style.opacity = 1f;

        yield return new WaitForSecondsRealtime(2f);

        saveConfirmationLabel.style.opacity = 0f;
        saveConfirmationRoutine = null;
    }

    void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == SceneNames.MainMenu) return;
        if (IsPaused) Resume(); else Pause();
    }

    public void Pause() => SetPaused(true);
    public void Resume() => SetPaused(false);

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        SetVisible(paused);
        UIHandler.Instance?.SetHUDVisible(!paused);
        PlayToggleSfx();
    }

    void PlayToggleSfx()
    {
        if (sfxSource != null && toggleClip != null)
            sfxSource.PlayOneShot(toggleClip);
    }

    void SetVisible(bool visible)
    {
        if (pauseRoot != null)
            pauseRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
