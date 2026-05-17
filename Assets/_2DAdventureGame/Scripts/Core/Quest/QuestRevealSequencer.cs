using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class QuestRevealSequencer : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera tourCamera;
    [SerializeField] private Transform tourTarget;

    [Header("Timing")]
    [SerializeField] private float lingerPerItem = 1.5f;
    [SerializeField] private float settleAfter = 2.0f;

    [Header("Priority")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int idlePriority = 0;

    private Coroutine running;

    void Awake() => tourCamera.Priority = idlePriority;

    void Start() => QuestManager.Instance.OnQuestAccepted += HandleAccepted;

    void OnDestroy()
    {
        if (QuestManager.Instance == null) return;
        QuestManager.Instance.OnQuestAccepted -= HandleAccepted;
    }

    void HandleAccepted(Quest quest)
    {
        List<Transform> targets = FindTargets(quest.Data);
        if (targets.Count == 0) return;

        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Reveal(targets));
    }

    List<Transform> FindTargets(QuestData data)
    {
        QuestBinding[] bindings = FindObjectsByType<QuestBinding>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<Transform> result = new();
        foreach (QuestBinding b in bindings)
            if (b.BoundQuest == data)
                result.Add(b.transform);
        return result;
    }

    IEnumerator Reveal(List<Transform> targets)
    {
        tourCamera.Priority = activePriority;

        foreach (Transform t in targets)
        {
            tourTarget.position = t.position;
            yield return new WaitForSeconds(lingerPerItem);
        }

        tourCamera.Priority = idlePriority;
        yield return new WaitForSeconds(settleAfter);

        running = null;
    }
}
