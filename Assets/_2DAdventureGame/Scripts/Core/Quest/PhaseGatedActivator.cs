using UnityEngine;

public class PhaseGatedActivator : MonoBehaviour
{
    [Tooltip("Quest whose phase gates the target's activation.")]
    [SerializeField] private QuestData quest;

    [Tooltip("GameObject to activate. May start disabled in the scene — its parent (this) must be on an always-active GameObject, separate from the target, so the gate logic can run.")]
    [SerializeField] private GameObject target;

    [Tooltip("If true, the target activates the first time the player concludes the after-phase dialogue during normal play. The target also activates automatically on scene load when the quest is already in After (so reloads behave correctly).")]
    [SerializeField] private bool activateOnConclude = true;

    private QuestController controller;

    void Awake()
    {
        if (target == gameObject)
            Debug.LogError($"[PhaseGatedActivator:{name}] 'target' points to this same GameObject. Move PhaseGatedActivator to a separate, always-active GameObject so it can gate a (potentially disabled) target.", this);
    }

    void Start()
    {
        controller = QuestManager.Instance != null ? QuestManager.Instance.Get(quest) : null;
        if (controller == null) return;

        if (controller.Phase == QuestPhase.After)
        {
            ActivateSilent();
            return;
        }

        if (activateOnConclude)
            controller.OnConcluded += HandleConcluded;
    }

    void OnDestroy()
    {
        if (controller != null)
            controller.OnConcluded -= HandleConcluded;
    }

    void HandleConcluded(QuestController _)
    {
        if (target != null) target.SetActive(true);
    }

    void ActivateSilent()
    {
        if (target == null) return;
        foreach (AudioSource source in target.GetComponentsInChildren<AudioSource>(true))
            source.playOnAwake = false;
        target.SetActive(true);
    }
}
