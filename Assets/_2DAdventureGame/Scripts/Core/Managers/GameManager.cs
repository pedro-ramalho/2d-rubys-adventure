using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private UIHandler uiHandler;
    [SerializeField] private float endGameDelay = 3f;

    private Enemy[] enemies;
    private int enemiesFixed = 0;
    private bool gameEnded = false;

    void Start()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
            enemy.OnFixed += HandleEnemyFixed;

        player.OnDied += HandlePlayerDied;
    }

    void OnDestroy()
    {
        foreach (Enemy enemy in enemies)
            enemy.OnFixed -= HandleEnemyFixed;

        player.OnDied -= HandlePlayerDied;
    }

    void HandlePlayerDied() => EndGame(win: false);
    void HandleEnemyFixed()
    {
        if (gameEnded) return;

        enemiesFixed++;

        if (enemiesFixed >= enemies.Length) EndGame(win: true);
    }

    void EndGame(bool win)
    {
        gameEnded = true;

        if (win)
            uiHandler.DisplayWinScreen();
        else
            uiHandler.DisplayLoseScreen();

        Invoke(nameof(ReloadScene), endGameDelay);
    }

    void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
