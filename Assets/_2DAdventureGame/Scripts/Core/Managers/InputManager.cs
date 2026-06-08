using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{
    private const string BindingsKey = "InputBindings";

    public PlayerInputActions Actions { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("InputManager");
        go.AddComponent<InputManager>();
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        Actions = new PlayerInputActions();
        LoadBindings();
        Actions.Player.Enable();
    }

    public void SaveBindings()
    {
        PlayerPrefs.SetString(BindingsKey, Actions.SaveBindingOverridesAsJson());
    }

    public void LoadBindings()
    {
        string json = PlayerPrefs.GetString(BindingsKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
            Actions.LoadBindingOverridesFromJson(json);
    }

    public void ResetBindings()
    {
        Actions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(BindingsKey);
    }
}
