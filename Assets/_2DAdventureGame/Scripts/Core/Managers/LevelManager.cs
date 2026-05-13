using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Load(int level)
    {
        SceneManager.LoadScene("Level " + level);
    }
}
