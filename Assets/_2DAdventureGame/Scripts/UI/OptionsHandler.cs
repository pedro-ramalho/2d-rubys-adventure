using System;
using UnityEngine;
using UnityEngine.Audio;
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
