using System;
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
        button.text = action.GetBindingDisplayString(bindingIndex);
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
                button.text = action.GetBindingDisplayString(bindingIndex);
                InputManager.Instance.SaveBindings();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                action.Enable();
                button.SetEnabled(true);
                button.text = action.GetBindingDisplayString(bindingIndex);
            })
            .Start();
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
