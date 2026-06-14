using UnityEngine;

public class QuestConclusionActivator : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;
    [SerializeField] private GameObject target;

    void Start()
    {
        if (QuestManager.Instance == null) return;

        QuestManager.Instance.OnQuestConcluded += HandleConcluded;

        if (QuestManager.Instance.IsCompleted(boundQuest) && target != null)
        {
            foreach (AudioSource source in target.GetComponentsInChildren<AudioSource>(true))
                source.playOnAwake = false;

            target.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestConcluded -= HandleConcluded;
    }

    void HandleConcluded(QuestData data)
    {
        if (data == boundQuest && target != null)
            target.SetActive(true);
    }
}
