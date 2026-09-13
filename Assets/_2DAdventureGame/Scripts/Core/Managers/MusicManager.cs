using System.Collections;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioSource stingerSource;
        [SerializeField] private float fadeOutDuration = 0.6f;
        [SerializeField] private float fadeInDuration = 0.6f;
        [SerializeField] private float transitionDelay = 1.0f;

        private Coroutine transition;
        private float baseVolume = 1f;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            if (source != null)
                baseVolume = source.volume;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null)
                return;

            if (source.clip == clip && source.isPlaying)
                return;

            if (transition != null)
                StopCoroutine(transition);

            if (source.clip == null)
            {
                source.clip = clip;
                source.loop = true;
                source.volume = baseVolume;
                source.Play();

                return;
            }

            transition = StartCoroutine(SwitchTo(clip));
        }

        public void PlayWithStinger(AudioClip stinger, AudioClip nextTrack)
        {
            if (stinger == null)
            {
                Play(nextTrack); 
            
                return;
            }

            if (transition != null) 
                StopCoroutine(transition);

            if (nextTrack == null)
                transition = StartCoroutine(StingerThenSilence(stinger));
            else
                transition = StartCoroutine(StingerThenTrack(stinger, nextTrack));
        }

        public void FadeOutAndStop(float duration)
        {
            if (transition != null) 
                StopCoroutine(transition);
        
            transition = StartCoroutine(FadeOutAndStopRoutine(duration));
        }

        IEnumerator FadeOutAndStopRoutine(float duration)
        {
            yield return FadeVolumeTo(0f, duration);
        
            source.Stop();
            source.clip = null;
            source.volume = baseVolume;
            transition = null;
        }

        IEnumerator SwitchTo(AudioClip clip)
        {
            yield return FadeVolumeTo(0f, fadeOutDuration);
        
            source.Stop();

            yield return new WaitForSecondsRealtime(transitionDelay);

            source.clip = clip;
            source.loop = true;
            source.Play();
        
            yield return FadeVolumeTo(baseVolume, fadeInDuration);

            transition = null;
        }

        IEnumerator StingerThenSilence(AudioClip stinger)
        {
            if (source.isPlaying)
            {
                yield return FadeVolumeTo(0f, fadeOutDuration);
            
                source.Stop();
                source.clip = null;
                source.volume = baseVolume;
            }

            if (stingerSource != null) stingerSource.PlayOneShot(stinger);
        
            yield return new WaitForSecondsRealtime(stinger.length);

            transition = null;
        }

        IEnumerator StingerThenTrack(AudioClip stinger, AudioClip nextTrack)
        {
            source.Stop();
        
            if (stingerSource != null) 
                stingerSource.PlayOneShot(stinger);

            yield return new WaitForSecondsRealtime(stinger.length);
            yield return new WaitForSecondsRealtime(transitionDelay);

            source.clip = nextTrack;
            source.loop = true;
            source.volume = 0f;
            source.Play();
        
            yield return FadeVolumeTo(baseVolume, fadeInDuration);

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
}
