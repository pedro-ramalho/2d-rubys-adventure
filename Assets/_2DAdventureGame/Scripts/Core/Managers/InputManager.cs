using UnityEngine;
using UnityEngine.InputSystem;

namespace AdventureGame.Core.Managers
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        private const string k_BindingsKey = "InputBindings";

        public PlayerInputActions Actions { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap() => BootstrapIfMissing();

        protected override void Awake()
        {
            base.Awake();
        
            if (Instance != this) return;

            Actions = new PlayerInputActions();
        
            LoadBindings();
        
            Actions.Player.Enable();
        }

        public void SaveBindings() => PlayerPrefs.SetString(k_BindingsKey, Actions.SaveBindingOverridesAsJson());
    
        public void LoadBindings()
        {
            string json = PlayerPrefs.GetString(k_BindingsKey, string.Empty);
            if (!string.IsNullOrEmpty(json))
                Actions.LoadBindingOverridesFromJson(json);
        }

        public void ResetBindings()
        {
            Actions.RemoveAllBindingOverrides();
        
            PlayerPrefs.DeleteKey(k_BindingsKey);
        }
    }
}
