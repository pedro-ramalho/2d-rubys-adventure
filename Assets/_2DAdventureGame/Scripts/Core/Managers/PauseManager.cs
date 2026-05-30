using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool IsPaused { get; private set; }

    [SerializeField] private UIDocument pauseDocument;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip toggleClip;

    private PlayerInputActions inputActions;
    private VisualElement pauseRoot;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Pause.performed += OnPausePressed;
    }

    void OnDisable()
    {
        inputActions.Player.Pause.performed -= OnPausePressed;
        inputActions.Player.Disable();
    }

    void Start()
    {
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
