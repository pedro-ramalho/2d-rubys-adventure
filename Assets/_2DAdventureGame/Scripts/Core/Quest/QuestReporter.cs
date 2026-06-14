using UnityEngine;

public class QuestReporter : MonoBehaviour
{
    [SerializeField] private QuestObjectiveType type;
    [SerializeField] private string reportTag;

    [Tooltip("Unique identifier for this specific scene object (e.g. 'l1q1.box.a'). Set on objects whose consumption must persist through save/reload. Leave empty for spawned/runtime entities that don't need per-instance persistence (e.g. wave-spawned enemies).")]
    [SerializeField] private string worldId;

    public string WorldId => worldId;

    public void Report()
    {
        Debug.Log($"[Reporter:{name}] Report() type={type} tag='{reportTag}' worldId='{worldId}'");
        QuestManager.Instance?.SubmitReport(new QuestReport(type, reportTag, worldId));
    }

    public bool IsAlreadyConsumed() =>
        !string.IsNullOrEmpty(worldId) &&
        QuestManager.Instance != null &&
        QuestManager.Instance.IsConsumed(worldId);
}
