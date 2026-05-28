using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private AudioSource source;
    [SerializeField] private float transitionDelay = 2.5f;

    public AudioClip[] Tracks => tracks;
    public int CurrentTrackIndex { get; private set; } = -1;

    private Coroutine transition;

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
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (source.clip == clip && source.isPlaying) return;

        CurrentTrackIndex = Array.IndexOf(tracks, clip);

        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(SwitchTo(clip));
    }

    public void Play(int index)
    {
        if (index < 0 || index >= tracks.Length) return;
        Play(tracks[index]);
    }

    IEnumerator SwitchTo(AudioClip clip)
    {
        source.Stop();
        yield return new WaitForSeconds(transitionDelay);

        source.clip = clip;
        source.loop = true;
        source.Play();

        transition = null;
    }
}
