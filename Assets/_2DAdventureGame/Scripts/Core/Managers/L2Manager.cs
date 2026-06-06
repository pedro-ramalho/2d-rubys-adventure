using UnityEngine;
using UnityEngine.SceneManagement;

public class L2Manager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private WaveSpawner spawner;
    [SerializeField] private UIHandler ui;
    [SerializeField] private QuestData winQuest;
    [SerializeField] private float endGameDelay = 3f;

    private bool gameEnded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.OnDied += HandlePlayerDied;
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestEpilogueFinished += HandleQuestEpilogueFinished;
    }

    void OnDestroy()
    {
        if (player != null)
            player.OnDied -= HandlePlayerDied;

        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestEpilogueFinished -= HandleQuestEpilogueFinished;
    }

    void HandlePlayerDied() => EndGame(win: false);

    void HandleQuestEpilogueFinished(QuestData data)
    {
        if (data == winQuest) EndGame(win: true);
    }

    void EndGame(bool win)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (win) 
            ui.DisplayWinScreen();
        else
            ui.DisplayLoseScreen();

        Invoke(nameof(ReloadScene), endGameDelay);
    }

    void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
