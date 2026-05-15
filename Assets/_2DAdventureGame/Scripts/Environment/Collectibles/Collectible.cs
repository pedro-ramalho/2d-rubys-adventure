using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField] protected AudioClip collectibleClip;
    public abstract void ApplyEffect(Player player);
    protected virtual void OnEffectApplied() => Destroy(gameObject);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            ApplyEffect(player);
            OnEffectApplied();
            GetComponent<QuestReporter>()?.Report();
        }
    }         
}
