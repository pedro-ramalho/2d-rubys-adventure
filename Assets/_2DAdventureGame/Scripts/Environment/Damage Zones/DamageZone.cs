using UnityEngine;

public class DamageZone : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
            damageable.ApplyDamage(amount: 1);
    }
}