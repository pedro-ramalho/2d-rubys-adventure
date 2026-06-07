using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseManager : PersistentSingleton<PauseManager>
{
    public static bool IsPaused { get; private set; }

    [SerializeField] private UIDocument pauseDocument;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip toggleClip;

    private PlayerInputActions inputActions;
    private VisualElement pauseRoot;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        if (Instance != this) return;
        inputActions.Player.Enable();
        inputActions.Player.Pause.performed += OnPausePressed;
    }

    void OnDisable()
    {
        if (Instance != this) return;
        inputActions.Player.Pause.performed -= OnPausePressed;
        inputActions.Player.Disable();
    }

    void Start()
    {
        if (Instance != this) return;
        pauseRoot = pauseDocument.rootVisualElement.Q<VisualElement>("PauseRoot");
        SetVisible(false);
    }

    void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (IsPaused) Resume(); else Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        SetVisible(true);
        PlayToggleSfx();
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SetVisible(false);
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
