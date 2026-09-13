using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Quest;
using AdventureGame.Core.Scene;
using AdventureGame.Core.Wave;
using AdventureGame.UI;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    public class ArenaManager : MonoBehaviour
    {
        [SerializeField] private WaveSpawner spawner;
        [SerializeField] private QuestData winQuest;
        [SerializeField] private float endGameDelay = 3f;
        [SerializeField] private float epilogueReadDelay = 3f;
        [SerializeField] private AudioSource stingerSource;
        [SerializeField] private AudioClip victoryStinger;

        private bool gameEnded;
        private QuestController controller;

        void Start()
        {
            spawner.OnAllWavesCleared += HandleAllWavesCleared;

            if (QuestManager.Instance != null)
            {
                controller = QuestManager.Instance.Get(winQuest);
                if (controller != null) controller.OnEpilogueFinished += HandleEpilogueFinished;
            }
        }

        void OnDestroy()
        {
            if (spawner != null) spawner.OnAllWavesCleared -= HandleAllWavesCleared;
            if (controller != null) controller.OnEpilogueFinished -= HandleEpilogueFinished;
        }

        void HandleAllWavesCleared()
        {
            if (controller != null) controller.MarkComplete();
        }

        void HandleEpilogueFinished(QuestController _) => StartCoroutine(DelayedWin());

        IEnumerator DelayedWin()
        {
            while (UIHandler.Instance != null && UIHandler.Instance.IsTyping)
                yield return null;
        
            yield return new WaitForSeconds(epilogueReadDelay);
        
            Win();
        }

        void Win()
        {
            if (gameEnded)
                return;

            gameEnded = true;

            MusicManager.Instance?.FadeOutAndStop(2f);
            if (stingerSource != null && victoryStinger != null)
                stingerSource.PlayOneShot(victoryStinger);

            EndScreenPresenter.Instance?.DisplayWinScreen();
        
            Invoke(nameof(ReloadScene), endGameDelay);
        }

        void ReloadScene() => SceneTransitioner.Instance?.LoadSceneWithCrossfade(SceneNames.MainMenu, 0f);
    }
}
