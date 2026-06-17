using UnityEngine;

public class SceneMusicConfigManager : MonoBehaviour
{
    public static SceneMusicConfigManager Instance { get; private set; }

    [SerializeField] private AudioClip defaultTrack;

    public AudioClip DefaultTrack => defaultTrack;

    void Awake() => Instance = this;

    void OnDestroy()
    {
        if (Instance == this) 
            Instance = null;    
    }

    void Start() => QuestMusic.Refresh();
}
