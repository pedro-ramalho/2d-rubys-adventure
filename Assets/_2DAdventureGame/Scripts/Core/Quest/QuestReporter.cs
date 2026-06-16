using UnityEngine;

public class QuestReporter : MonoBehaviour
{
    [SerializeField] private QuestData quest;

    [Tooltip("Unique identifier for this specific scene object (e.g. 'l1q1.box.a'). Required for counted-quest contributors whose collection state must persist through save/reload.")]
    [SerializeField] private string worldId;

    public QuestData Quest => quest;
    public string WorldId => worldId;

    public void Report()
    {
        CountedQuestController controller = GetCountedController();
        if (controller != null) controller.TryReport(worldId);
    }

    public bool CanReport()
    {
        CountedQuestController controller = GetCountedController();
        return controller != null && controller.CanReport(worldId);
    }

    public bool IsConsumed()
    {
        CountedQuestController controller = GetCountedController();
        return controller != null && controller.IsConsumed(worldId);
    }

    CountedQuestController GetCountedController() =>
        QuestManager.Instance != null ? QuestManager.Instance.Get(quest) as CountedQuestController : null;
}
