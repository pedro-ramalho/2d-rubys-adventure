using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool IsPaused { get; private set; }

    [SerializeField] private UIDocument pauseDocument;

    private PlayerInputActions inputActions;
    private VisualElement pauseRoot;
    private DropdownField musicDropdown;
    private Slider volumeSlider;
    private Button resumeButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
        VisualElement root = pauseDocument.rootVisualElement;
        pauseRoot = root.Q<VisualElement>("PauseRoot");
        musicDropdown = root.Q("MusicDropdown") as DropdownField;
        volumeSlider = root.Q<Slider>("VolumeSlider");
        resumeButton = root.Q<Button>("ResumeButton");

        SetVisible(false);

        if (MusicManager.Instance != null && musicDropdown != null)
        {
            List<string> names = new List<string>();
            foreach (AudioClip clip in MusicManager.Instance.Tracks) names.Add(clip.name);

            musicDropdown.choices = names;
            musicDropdown.index = MusicManager.Instance.CurrentTrackIndex;
            musicDropdown.RegisterValueChangedCallback(evt =>
                MusicManager.Instance.Play(musicDropdown.index));
        }

        if (MusicManager.Instance != null && volumeSlider != null)
        {
            volumeSlider.value = MusicManager.Instance.Volume;
            volumeSlider.RegisterValueChangedCallback(evt =>
                MusicManager.Instance.Volume = evt.newValue);
        }

        if (resumeButton != null) resumeButton.clicked += Resume;
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
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        if (pauseRoot != null)
            pauseRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
