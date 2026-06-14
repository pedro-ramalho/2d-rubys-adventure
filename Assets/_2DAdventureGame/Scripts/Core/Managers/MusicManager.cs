using System.Collections;
using UnityEngine;

public class MusicManager : PersistentSingleton<MusicManager>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource stingerSource;
    [SerializeField] private float fadeOutDuration = 0.6f;
    [SerializeField] private float fadeInDuration = 0.6f;
    [SerializeField] private float transitionDelay = 1.0f;

    private Coroutine transition;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        if (source != null) source.ignoreListenerPause = true;
        if (stingerSource != null) stingerSource.ignoreListenerPause = true;
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (source.clip == clip && source.isPlaying) return;

        if (transition != null) StopCoroutine(transition);

        if (source.clip == null)
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
            return;
        }

        transition = StartCoroutine(SwitchTo(clip));
    }

    public void PlayWithStinger(AudioClip stinger, AudioClip nextTrack)
    {
        if (stinger == null) { Play(nextTrack); return; }
        if (nextTrack == null) return;

        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(StingerThenTrack(stinger, nextTrack));
    }

    public void FadeOutAndStop(float duration)
    {
        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(FadeOutAndStopRoutine(duration));
    }

    IEnumerator FadeOutAndStopRoutine(float duration)
    {
        float restoreVolume = source.volume;
        yield return FadeVolumeTo(0f, duration);
        source.Stop();
        source.clip = null;
        source.volume = restoreVolume;
        transition = null;
    }

    IEnumerator SwitchTo(AudioClip clip)
    {
        float originalVolume = source.volume;
        yield return FadeVolumeTo(0f, fadeOutDuration);
        source.Stop();

        yield return new WaitForSecondsRealtime(transitionDelay);

        source.clip = clip;
        source.loop = true;
        source.Play();
        yield return FadeVolumeTo(originalVolume, fadeInDuration);

        transition = null;
    }

    IEnumerator StingerThenTrack(AudioClip stinger, AudioClip nextTrack)
    {
        float originalVolume = source.volume;

        source.Stop();
        if (stingerSource != null) stingerSource.PlayOneShot(stinger);

        yield return new WaitForSecondsRealtime(stinger.length);
        yield return new WaitForSecondsRealtime(transitionDelay);

        source.clip = nextTrack;
        source.loop = true;
        source.volume = 0f;
        source.Play();
        yield return FadeVolumeTo(originalVolume, fadeInDuration);

        transition = null;
    }

    IEnumerator FadeVolumeTo(float target, float duration)
    {
        if (duration <= 0f)
        {
            source.volume = target;
            yield break;
        }

        float start = source.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        source.volume = target;
    }
}
