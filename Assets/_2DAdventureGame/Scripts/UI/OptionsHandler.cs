using System;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OptionsHandler : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private const float DefaultVolume = 1f;
    private const float MinDb = -80f;

    public event Action Opened;
    public event Action Closed;

    private VisualElement optionsRoot;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        optionsRoot = root.Q<VisualElement>("OptionsRoot");

        Button backButton = root.Q<Button>("OptionsBackButton");
        backButton.clicked += Close;

        WireVolumeSlider(root, "MasterVolumeSlider", "MasterVolume");
        WireVolumeSlider(root, "MusicVolumeSlider", "MusicVolume");
        WireVolumeSlider(root, "SfxVolumeSlider", "SfxVolume");
        WireVolumeSlider(root, "AmbientVolumeSlider", "AmbientVolume");

        PlayerInputActions actions = InputManager.Instance.Actions;
        WireRebindButton(root, "DashRebindButton", actions.Player.Dash, 0);
        WireRebindButton(root, "ShootRebindButton", actions.Player.Shoot, 0);

        WireMovementRebind(root, "Up", 1, 6);
        WireMovementRebind(root, "Down", 2, 7);
        WireMovementRebind(root, "Left", 3, 8);
        WireMovementRebind(root, "Right", 4, 9);
    }

    void WireMovementRebind(VisualElement root, string direction, int primaryIndex, int secondaryIndex)
    {
        InputAction movement = InputManager.Instance.Actions.Player.Movement;
        WireRebindButton(root, $"{direction}PrimaryRebindButton", movement, primaryIndex);
        WireRebindButton(root, $"{direction}SecondaryRebindButton", movement, secondaryIndex);
    }

    void WireVolumeSlider(VisualElement root, string sliderName, string mixerParam)
    {
        Slider slider = root.Q<Slider>(sliderName);
        float saved = PlayerPrefs.GetFloat(mixerParam, DefaultVolume);
        slider.SetValueWithoutNotify(saved);
        ApplyVolume(mixerParam, saved);

        slider.RegisterValueChangedCallback(evt =>
        {
            ApplyVolume(mixerParam, evt.newValue);
            PlayerPrefs.SetFloat(mixerParam, evt.newValue);
        });
    }

    void ApplyVolume(string mixerParam, float linear)
    {
        float dB = linear > 0.0001f ? Mathf.Log10(linear) * 20f : MinDb;
        audioMixer.SetFloat(mixerParam, dB);
    }

    void WireRebindButton(VisualElement root, string buttonName, InputAction action, int bindingIndex)
    {
        Button button = root.Q<Button>(buttonName);
        button.text = GetBindingDisplayName(action, bindingIndex);
        button.clicked += () => StartRebind(action, bindingIndex, button);
    }

    void StartRebind(InputAction action, int bindingIndex, Button button)
    {
        action.Disable();
        button.SetEnabled(false);
        button.text = "Press a key...";

        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(op =>
            {
                op.Dispose();
                action.Enable();
                button.SetEnabled(true);
                button.text = GetBindingDisplayName(action, bindingIndex);
                InputManager.Instance.SaveBindings();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                action.Enable();
                button.SetEnabled(true);
                button.text = GetBindingDisplayName(action, bindingIndex);
            })
            .Start();
    }

    static string GetBindingDisplayName(InputAction action, int bindingIndex)
    {
        string path = action.bindings[bindingIndex].effectivePath;
        if (string.IsNullOrEmpty(path)) return string.Empty;
        int slashIdx = path.LastIndexOf('/');
        string keyName = slashIdx >= 0 ? path.Substring(slashIdx + 1) : path;
        return FormatKeyName(keyName);
    }

    static string FormatKeyName(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;
        StringBuilder sb = new StringBuilder();
        sb.Append(char.ToUpperInvariant(raw[0]));
        for (int i = 1; i < raw.Length; i++)
        {
            if (char.IsUpper(raw[i])) sb.Append(' ');
            sb.Append(raw[i]);
        }
        return sb.ToString();
    }

    public void Open()
    {
        optionsRoot.style.display = DisplayStyle.Flex;
        Opened?.Invoke();
    }

    public void Close()
    {
        optionsRoot.style.display = DisplayStyle.None;
        Closed?.Invoke();
    }
}
