using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Managers;
using AdventureGame.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Scene
{
    public class SceneTransitioner : SceneSingleton<SceneTransitioner>
    {
        [SerializeField]
        private Animator m_TransitionAnimator;

        [SerializeField]
        private string m_StartTrigger = "Start";

        [SerializeField]
        private string m_FadeOutClipName = "CrossfadeStart_Animation";

        [SerializeField]
        private float m_PreTransitionDelay = 1.5f;

        private float m_FadeOutDuration = 1f;

        public bool IsTransitioning { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this)
                return;

            m_FadeOutDuration = ResolveClipLength(m_FadeOutClipName);
        }

        float ResolveClipLength(string clipName)
        {
            if (
                m_TransitionAnimator == null
                || m_TransitionAnimator.runtimeAnimatorController == null
            )
                return 1f;

            foreach (var clip in m_TransitionAnimator.runtimeAnimatorController.animationClips)
                if (clip.name == clipName)
                    return clip.length;

            return 1f;
        }

        public void LoadSceneWithCrossfade(string sceneName) =>
            LoadSceneWithCrossfade(sceneName, m_PreTransitionDelay, writeSave: true);

        public void LoadSceneWithCrossfade(string sceneName, float preDelay) =>
            LoadSceneWithCrossfade(sceneName, preDelay, writeSave: true);

        public void LoadSceneWithCrossfade(string sceneName, float preDelay, bool writeSave) =>
            StartCoroutine(LoadScene(sceneName, preDelay, writeSave));

        IEnumerator LoadScene(string scene, float preDelay, bool writeSave)
        {
            IsTransitioning = true;

            if (MusicManager.Instance != null)
                MusicManager.Instance.FadeOutAndStop(preDelay + m_FadeOutDuration);

            yield return new WaitForSeconds(preDelay);

            if (DialoguePresenter.Instance != null)
                DialoguePresenter.Instance.HideDialogue();

            if (HealthBarHUD.Instance != null)
                HealthBarHUD.Instance.Hide();

            m_TransitionAnimator.SetTrigger(m_StartTrigger);

            yield return new WaitForSeconds(m_FadeOutDuration);

            if (writeSave && scene != SceneNames.MainMenu && SaveManager.Instance != null)
                SaveManager.Instance.WriteSave(scene);

            SceneManager.LoadScene(scene);
        }
    }
}
