using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField] protected AudioClip collectibleClip;
    public abstract void ApplyEffect(Collider2D other);
    protected virtual void OnEffectApplied() => Destroy(gameObject);

    void OnTriggerEnter2D(Collider2D other)
    {
        ApplyEffect(other);
        
        if (other.TryGetComponent(out Player _))
            OnEffectApplied();
    }         
}
