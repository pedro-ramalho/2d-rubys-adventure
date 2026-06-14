using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] protected AudioClip collectibleClip;
    protected virtual void ApplyEffect(Player player) { }
    protected virtual void OnEffectApplied() => Destroy(gameObject);

    void Start()
    {
        QuestReporter reporter = GetComponent<QuestReporter>();
        bool consumed = reporter != null && reporter.IsAlreadyConsumed();
        Debug.Log($"[Collect:{name}] Start. reporter={(reporter != null ? "yes" : "no")} worldId='{(reporter != null ? reporter.WorldId : "")}' IsAlreadyConsumed={consumed}");
        if (consumed)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Debug.Log($"[Collect:{name}] OnTriggerEnter2D by player");
            ApplyEffect(player);

            if (collectibleClip != null)
                player.OneShotSource.PlayOneShot(collectibleClip);

            OnEffectApplied();
            GetComponent<QuestReporter>()?.Report();
        }
    }
}
