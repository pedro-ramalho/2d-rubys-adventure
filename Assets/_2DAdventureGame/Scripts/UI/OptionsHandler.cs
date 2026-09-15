using System;
using System.Text;
using AdventureGame.Core.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class OptionsHandler : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer m_AudioMixer;

        private const float k_DefaultVolume = 1f;
        private const float k_MinDb = -80f;

        private static readonly (string Slider, string Param)[] VolumeEntries =
        {
            ("MasterVolumeSlider", "MasterVolume"),
            ("MusicVolumeSlider", "MusicVolume"),
            ("SfxVolumeSlider", "SfxVolume"),
            ("AmbientVolumeSlider", "AmbientVolume"),
        };

        public event Action Opened;
        public event Action Closed;

        private VisualElement m_OptionsRoot;
        private (string Button, InputAction Action, int Index)[] rebindEntries;
        private MainMenuHandler m_MainMenuHandler;
        private bool m_IsRebinding;

        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            m_OptionsRoot = root.Q<VisualElement>("OptionsRoot");
            m_MainMenuHandler = GetComponent<MainMenuHandler>();

            Button backButton = root.Q<Button>("OptionsBackButton");
            backButton.clicked += PlayClick;
            backButton.clicked += Close;

            Button deleteSaveButton = root.Q<Button>("DeleteSaveButton");
            deleteSaveButton.clicked += PlayClick;
            deleteSaveButton.clicked += DeleteSave;

            Button resetButton = root.Q<Button>("ResetDefaultsButton");
            resetButton.clicked += PlayClick;
            resetButton.clicked += ResetToDefaults;

            foreach ((string slider, string param) in VolumeEntries)
                WireVolumeSlider(root, slider, param);

            PlayerInputActions a = InputManager.Instance.Actions;
            InputAction move = a.Player.Movement;

            rebindEntries = new (string, InputAction, int)[]
            {
                ("DashRebindButton", a.Player.Dash, 0),
                ("ShootRebindButton", a.Player.Shoot, 0),
                ("UpPrimaryRebindButton", move, 1),
                ("DownPrimaryRebindButton", move, 2),
                ("LeftPrimaryRebindButton", move, 3),
                ("RightPrimaryRebindButton", move, 4),
                ("UpSecondaryRebindButton", move, 6),
                ("DownSecondaryRebindButton", move, 7),
                ("LeftSecondaryRebindButton", move, 8),
                ("RightSecondaryRebindButton", move, 9),
            };

            foreach ((string button, InputAction action, int index) in rebindEntries)
                WireRebindButton(root, button, action, index);
        }

        void ResetToDefaults()
        {
            foreach ((string slider, string param) in VolumeEntries)
                ResetVolumeSlider(slider, param);

            InputManager.Instance.ResetBindings();

            foreach ((string button, InputAction action, int index) in rebindEntries)
                RefreshRebindLabel(button, action, index);
        }

        void ResetVolumeSlider(string sliderName, string mixerParam)
        {
            Slider slider = m_OptionsRoot.Q<Slider>(sliderName);
            slider.SetValueWithoutNotify(k_DefaultVolume);

            ApplyVolume(mixerParam, k_DefaultVolume);
            PlayerPrefs.DeleteKey(mixerParam);
        }

        void RefreshRebindLabel(string buttonName, InputAction action, int bindingIndex)
        {
            Button button = m_OptionsRoot.Q<Button>(buttonName);
            button.text = GetBindingDisplayName(action, bindingIndex);
        }

        void WireVolumeSlider(VisualElement root, string sliderName, string mixerParam)
        {
            Slider slider = root.Q<Slider>(sliderName);
            float saved = PlayerPrefs.GetFloat(mixerParam, k_DefaultVolume);
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
            float dB = linear > 0.0001f ? Mathf.Log10(linear) * 20f : k_MinDb;
            m_AudioMixer.SetFloat(mixerParam, dB);
        }

        void WireRebindButton(
            VisualElement root,
            string buttonName,
            InputAction action,
            int bindingIndex
        )
        {
            Button button = root.Q<Button>(buttonName);
            button.text = GetBindingDisplayName(action, bindingIndex);
            button.clicked += PlayClick;
            button.clicked += () => StartRebind(action, bindingIndex, button);
        }

        void PlayClick() => m_MainMenuHandler?.PlayClick();

        void StartRebind(InputAction action, int bindingIndex, Button button)
        {
            if (m_IsRebinding)
                return;
            m_IsRebinding = true;

            action.Disable();
            button.SetEnabled(false);
            button.text = "Press a key...";

            action
                .PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnComplete(op =>
                {
                    op.Dispose();
                    action.Enable();
                    button.SetEnabled(true);
                    button.text = GetBindingDisplayName(action, bindingIndex);
                    InputManager.Instance.SaveBindings();
                    m_IsRebinding = false;
                })
                .OnCancel(op =>
                {
                    op.Dispose();
                    action.Enable();
                    button.SetEnabled(true);
                    button.text = GetBindingDisplayName(action, bindingIndex);
                    m_IsRebinding = false;
                })
                .Start();
        }

        static string GetBindingDisplayName(InputAction action, int bindingIndex)
        {
            string path = action.bindings[bindingIndex].effectivePath;
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            int slashIdx = path.LastIndexOf('/');
            string keyName = slashIdx >= 0 ? path.Substring(slashIdx + 1) : path;

            return FormatKeyName(keyName);
        }

        static string FormatKeyName(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return raw;

            StringBuilder sb = new StringBuilder();
            sb.Append(char.ToUpperInvariant(raw[0]));
            for (int i = 1; i < raw.Length; i++)
            {
                if (char.IsUpper(raw[i]))
                    sb.Append(' ');
                sb.Append(raw[i]);
            }

            return sb.ToString();
        }

        public void Open()
        {
            m_OptionsRoot.style.display = DisplayStyle.Flex;

            RefreshDeleteSaveButton();

            Opened?.Invoke();
        }

        void DeleteSave()
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.DeleteSave();

            RefreshDeleteSaveButton();
        }

        void RefreshDeleteSaveButton()
        {
            Button button = m_OptionsRoot.Q<Button>("DeleteSaveButton");

            bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSave;

            button.SetEnabled(hasSave);
        }

        public void Close()
        {
            m_OptionsRoot.style.display = DisplayStyle.None;

            Closed?.Invoke();
        }
    }
}
