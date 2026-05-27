using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private AudioSource source;

    public AudioClip[] Tracks => tracks;
    public int CurrentTrackIndex { get; private set; } = -1;

    public float Volume
    {
        get => source.volume;
        set => source.volume = Mathf.Clamp01(value);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (tracks.Length > 0) Play(0);
    }

    public void Play(int index)
    {
        if (index < 0 || index >= tracks.Length) return;

        CurrentTrackIndex = index;
        source.clip = tracks[index];
        source.loop = true;
        source.Play();
    }
}
