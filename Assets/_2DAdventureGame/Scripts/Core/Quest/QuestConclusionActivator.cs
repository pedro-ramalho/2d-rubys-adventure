using UnityEngine;

public class QuestConclusionActivator : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;
    [SerializeField] private GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestConcluded += HandleConcluded;    
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
