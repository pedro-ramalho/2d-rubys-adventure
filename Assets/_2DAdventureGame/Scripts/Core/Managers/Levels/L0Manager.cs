using UnityEngine;
using UnityEngine.SceneManagement;

public class L0Manager : MonoBehaviour
{
    [SerializeField] private NPC targetNpc;
    [SerializeField] private QuestData triggerQuest;
    [SerializeField] private string nextSceneName = "Level 1";

    void Start() => targetNpc.OnDialogueExhausted += HandleDialogueExhausted;

    void OnDestroy()
    {
        if (targetNpc != null) targetNpc.OnDialogueExhausted -= HandleDialogueExhausted;
    }

    void HandleDialogueExhausted(QuestDialogue dialogue)
    {
        if (dialogue.quest == triggerQuest && QuestManager.Instance.IsCompleted(triggerQuest))
            LoadNextLevel();
    }

    public void LoadNextLevel() => SceneManager.LoadScene(nextSceneName);
}
