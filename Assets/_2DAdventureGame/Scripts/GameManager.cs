using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    EnemyController[] enemies;
    public UIHandler uiHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
