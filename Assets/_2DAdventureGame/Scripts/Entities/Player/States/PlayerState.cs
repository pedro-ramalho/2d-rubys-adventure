using UnityEngine;

public abstract class PlayerState : State<Player>
{
    public override void Enter(Player owner) { }
    public override void Update(Player owner) { }
    public override void FixedUpdate(Player owner) { }
    public override void Exit(Player owner) { }

    public virtual void HandleHeal(Player owner, int amount)
    {
        owner.CurrentHealth = Mathf.Clamp(owner.CurrentHealth + amount, 0, owner.Data.maxHealth);
        owner.RaiseOnHealthChanged(owner.CurrentHealth / (float)owner.Data.maxHealth);
    }

    public virtual void HandleDamage(Player owner, int amount)
    {
        if (owner.IsInvincible) return;

        owner.IsInvincible = true;
        owner.DamageCooldown = owner.Data.invincibilityDuration;

        owner.Animator.SetTrigger(Player.HitHash);
        owner.OneShotSource.PlayOneShot(owner.HitClip);
        
        CameraShake.Instance?.Shake();

        owner.CurrentHealth = Mathf.Clamp(owner.CurrentHealth - amount, 0, owner.Data.maxHealth);
        owner.RaiseOnHealthChanged(owner.CurrentHealth / (float)owner.Data.maxHealth);

        if (owner.CurrentHealth == 0)
        {
            owner.RaiseOnDied();
            owner.ChangeState(owner.DeadState);
        }
    }
}
