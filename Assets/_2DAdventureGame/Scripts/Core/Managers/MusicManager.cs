using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Managers
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private AudioSource m_StingerAudioSource;

        [SerializeField]
        private float m_FadeOutDurationS = 0.6f;

        [SerializeField]
        private float m_FadeInDurationS = 0.6f;

        [SerializeField]
        private float m_TransitionDelayS = 1.0f;

        private Coroutine m_TransitionCoroutine;
        private float m_BaseVolume = 1f;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            if (m_AudioSource != null)
                m_BaseVolume = m_AudioSource.volume;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null)
                return;

            if (m_AudioSource.clip == clip && m_AudioSource.isPlaying)
                return;

            if (m_TransitionCoroutine != null)
                StopCoroutine(m_TransitionCoroutine);

            if (m_AudioSource.clip == null)
            {
                m_AudioSource.clip = clip;
                m_AudioSource.loop = true;
                m_AudioSource.volume = m_BaseVolume;
                m_AudioSource.Play();

                return;
            }

            m_TransitionCoroutine = StartCoroutine(SwitchTo(clip));
        }

        public void PlayWithStinger(AudioClip stinger, AudioClip nextTrack)
        {
            if (stinger == null)
            {
                Play(nextTrack);

                return;
            }

            if (m_TransitionCoroutine != null)
                StopCoroutine(m_TransitionCoroutine);

            if (nextTrack == null)
                m_TransitionCoroutine = StartCoroutine(StingerThenSilence(stinger));
            else
                m_TransitionCoroutine = StartCoroutine(StingerThenTrack(stinger, nextTrack));
        }

        public void FadeOutAndStop(float duration)
        {
            if (m_TransitionCoroutine != null)
                StopCoroutine(m_TransitionCoroutine);

            m_TransitionCoroutine = StartCoroutine(FadeOutAndStopRoutine(duration));
        }

        IEnumerator FadeOutAndStopRoutine(float duration)
        {
            yield return FadeVolumeTo(0f, duration);

            m_AudioSource.Stop();
            m_AudioSource.clip = null;
            m_AudioSource.volume = m_BaseVolume;
            m_TransitionCoroutine = null;
        }

        IEnumerator SwitchTo(AudioClip clip)
        {
            yield return FadeVolumeTo(0f, m_FadeOutDurationS);

            m_AudioSource.Stop();

            yield return new WaitForSecondsRealtime(m_TransitionDelayS);

            m_AudioSource.clip = clip;
            m_AudioSource.loop = true;
            m_AudioSource.Play();

            yield return FadeVolumeTo(m_BaseVolume, m_FadeInDurationS);

            m_TransitionCoroutine = null;
        }

        IEnumerator StingerThenSilence(AudioClip stinger)
        {
            if (m_AudioSource.isPlaying)
            {
                yield return FadeVolumeTo(0f, m_FadeOutDurationS);

                m_AudioSource.Stop();
                m_AudioSource.clip = null;
                m_AudioSource.volume = m_BaseVolume;
            }

            if (m_StingerAudioSource != null)
                m_StingerAudioSource.PlayOneShot(stinger);

            yield return new WaitForSecondsRealtime(stinger.length);

            m_TransitionCoroutine = null;
        }

        IEnumerator StingerThenTrack(AudioClip stinger, AudioClip nextTrack)
        {
            m_AudioSource.Stop();

            if (m_StingerAudioSource != null)
                m_StingerAudioSource.PlayOneShot(stinger);

            yield return new WaitForSecondsRealtime(stinger.length);
            yield return new WaitForSecondsRealtime(m_TransitionDelayS);

            m_AudioSource.clip = nextTrack;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0f;
            m_AudioSource.Play();

            yield return FadeVolumeTo(m_BaseVolume, m_FadeInDurationS);

            m_TransitionCoroutine = null;
        }

        IEnumerator FadeVolumeTo(float target, float duration)
        {
            if (duration <= 0f)
            {
                m_AudioSource.volume = target;

                yield break;
            }

            float start = m_AudioSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                m_AudioSource.volume = Mathf.Lerp(start, target, elapsed / duration);

                yield return null;
            }

            m_AudioSource.volume = target;
        }
    }
}
