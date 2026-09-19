using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Quests;
using AdventureGame.Core.Scene;
using AdventureGame.Core.Wave;
using AdventureGame.Entities.NPC;
using AdventureGame.UI;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    public class ArenaManager : MonoBehaviour
    {
        [SerializeField]
        private WaveSpawner m_WaveSpawner;

        [SerializeField]
        private QuestDefinition m_QuestData;

        [SerializeField]
        private float m_EndGameDelayS = 3f;

        [SerializeField]
        private float m_EpilogueReadDelayS = 3f;

        [SerializeField]
        private AudioSource m_StingerAudioSource;

        [SerializeField]
        private AudioClip m_VictoryStingerSfx;

        private bool m_IsGameOver;
        private Quest m_Quest;

        void Start()
        {
            m_WaveSpawner.OnAllWavesCleared += HandleAllWavesCleared;

            NPC.OnEpilogueEnded += HandleEpilogueEnded;

            if (QuestManager.Instance != null)
                m_Quest = QuestManager.Instance.Get(m_QuestData);
        }

        void OnDestroy()
        {
            if (m_WaveSpawner != null)
                m_WaveSpawner.OnAllWavesCleared -= HandleAllWavesCleared;

            NPC.OnEpilogueEnded -= HandleEpilogueEnded;
        }

        void HandleAllWavesCleared()
        {
            if (m_Quest != null)
                m_Quest.MarkComplete();
        }

        void HandleEpilogueEnded(QuestDefinition data)
        {
            if (data == m_QuestData)
                StartCoroutine(DelayedWin());
        }

        IEnumerator DelayedWin()
        {
            while (DialoguePresenter.Instance != null && DialoguePresenter.Instance.IsTyping)
                yield return null;

            yield return new WaitForSeconds(m_EpilogueReadDelayS);

            Win();
        }

        void Win()
        {
            if (m_IsGameOver)
                return;

            m_IsGameOver = true;

            MusicManager.Instance?.FadeOutAndStop(2f);
            if (m_StingerAudioSource != null && m_VictoryStingerSfx != null)
                m_StingerAudioSource.PlayOneShot(m_VictoryStingerSfx);

            if (EndScreenPresenter.Instance != null)
                EndScreenPresenter.Instance.DisplayWinScreen();

            Invoke(nameof(ReloadScene), m_EndGameDelayS);
        }

        void ReloadScene() =>
            SceneTransitioner.Instance?.LoadSceneWithCrossfade(SceneNames.MainMenu, 0f);
    }
}
