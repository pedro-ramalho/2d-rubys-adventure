using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] private WaveSpawner spawner;
    [SerializeField] private UIHandler ui;
    [SerializeField] private Marshmallow marshmallow;
    [SerializeField] private QuestData winQuest;
    [SerializeField] private float endGameDelay = 3f;
    [SerializeField] private float epilogueReadDelay = 3f;
    [SerializeField] private AudioSource stingerSource;
    [SerializeField] private AudioClip victoryStinger;

    private bool gameEnded;

    void Start()
    {
        spawner.OnAllWavesCleared += HandleAllWavesCleared;
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestEpilogueFinished += HandleQuestEpilogueFinished;
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnAllWavesCleared -= HandleAllWavesCleared;

        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestEpilogueFinished -= HandleQuestEpilogueFinished;
    }

    void HandleAllWavesCleared()
    {
        if (marshmallow != null) marshmallow.WalkBack();
    }

    void HandleQuestEpilogueFinished(QuestData data)
    {
        if (data == winQuest) StartCoroutine(DelayedWin());
    }

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

        ui.DisplayWinScreen();
        Invoke(nameof(ReloadScene), endGameDelay);
    }

    void ReloadScene() => SceneTransitioner.Instance?.LoadSceneWithCrossfade(SceneNames.MainMenu, 0f);
}
