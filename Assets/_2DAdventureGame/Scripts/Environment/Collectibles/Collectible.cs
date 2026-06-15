using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] protected AudioClip collectibleClip;
    protected virtual void ApplyEffect(Player player) { }
    protected virtual void OnEffectApplied() => Destroy(gameObject);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            ApplyEffect(player);

            if (collectibleClip != null)
                player.OneShotSource.PlayOneShot(collectibleClip);

            OnEffectApplied();
            GetComponent<QuestReporter>()?.Report();
        }
    }         
}
