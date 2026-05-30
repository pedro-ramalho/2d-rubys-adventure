using UnityEngine;

public class SceneMusicConfigManager : MonoBehaviour
{
    public static SceneMusicConfigManager Instance { get; private set; }

    [SerializeField] private AudioClip defaultTrack;

    public AudioClip DefaultTrack => defaultTrack;

    void Awake() => Instance = this;

    void OnDestroy()
    {
        if (Instance == this) Instance = null;    
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MusicManager.Instance != null && defaultTrack != null)
            MusicManager.Instance.Play(defaultTrack);
    }
}
