using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public static bool Active => Instance != null && Instance.IsActive;

    public bool IsActive { get; set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
