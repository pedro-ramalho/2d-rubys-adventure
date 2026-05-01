using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIHandler uiHandler;

    EnemyController[] enemies;
    int enemiesFixed = 0;
    bool gameEnded = false;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in enemies)
        {
            enemy.OnFixed += HandleEnemyFixed;
        }
    }

    void Update()
    {
        if (gameEnded) return;

        if (player.health <= 0)
        {
            EndGame(win: false);
        }
    }

    void HandleEnemyFixed()
    {
        if (gameEnded) return;

        enemiesFixed++;

        if (enemiesFixed >= enemies.Length)
        {
            EndGame(win: true);
        }
    }

    void EndGame(bool win)
    {
        gameEnded = true;

        if (win)
            uiHandler.DisplayWinScreen();
        else
            uiHandler.DisplayLoseScreen();

        Invoke(nameof(ReloadScene), 3f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
