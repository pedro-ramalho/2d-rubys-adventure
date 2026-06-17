using UnityEngine;

public class PhaseGatedActivator : MonoBehaviour
{
    [SerializeField] private QuestData quest;

    [SerializeField] private GameObject target;

    [SerializeField] private bool activateOnConclude = true;

    private QuestController controller;

    void Start()
    {
        controller = QuestManager.Instance != null ? QuestManager.Instance.Get(quest) : null;
        if (controller == null) 
            return;

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
        if (target != null) 
            target.SetActive(true);
    }

    void ActivateSilent()
    {
        if (target == null) 
            return;
        
        foreach (AudioSource source in target.GetComponentsInChildren<AudioSource>(true))
            source.playOnAwake = false;
        
        target.SetActive(true);
    }
}
