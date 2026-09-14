using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Quests;
using AdventureGame.Core.Scene;
using AdventureGame.Core.Wave;
using AdventureGame.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Managers
{
    public class ArenaManager : MonoBehaviour
    {
        [FormerlySerializedAs("spawner")]
        [SerializeField] private WaveSpawner m_WaveSpawner;

        [FormerlySerializedAs("winQuest")]
        [SerializeField] private QuestData m_QuestData;

        [FormerlySerializedAs("endGameDelay")]
        [SerializeField] private float m_EndGameDelayS = 3f;

        [FormerlySerializedAs("epilogueReadDelay")]
        [SerializeField] private float m_EpilogueReadDelayS = 3f;
        
        [FormerlySerializedAs("stingerSource")]
        [SerializeField] private AudioSource m_StingerAudioSource;

        [FormerlySerializedAs("victoryStinger")]
        [SerializeField] private AudioClip m_VictoryStingerSfx;

        private bool m_IsGameOver;
        private Quest m_Quest;

        void Start()
        {
            m_WaveSpawner.OnAllWavesCleared += HandleAllWavesCleared;

            if (QuestManager.Instance != null)
            {
                m_Quest = QuestManager.Instance.Get(m_QuestData);
                if (m_Quest != null) m_Quest.OnEpilogueFinished += HandleEpilogueFinished;
            }
        }

        void OnDestroy()
        {
            if (m_WaveSpawner != null) m_WaveSpawner.OnAllWavesCleared -= HandleAllWavesCleared;
            if (m_Quest != null) m_Quest.OnEpilogueFinished -= HandleEpilogueFinished;
        }

        void HandleAllWavesCleared()
        {
            if (m_Quest != null) m_Quest.MarkComplete();
        }

        void HandleEpilogueFinished(Quest _) => StartCoroutine(DelayedWin());

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

        void ReloadScene() => SceneTransitioner.Instance?.LoadSceneWithCrossfade(SceneNames.MainMenu, 0f);
    }
}
