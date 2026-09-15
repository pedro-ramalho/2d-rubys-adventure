using AdventureGame.Core;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Effects;
using UnityEngine;

namespace AdventureGame.Entities.Player.States
{
    public abstract class PlayerState : State<Player>
    {
        public override void Enter(Player owner) { }

        public override void Update(Player owner) { }

        public override void FixedUpdate(Player owner) { }

        public override void Exit(Player owner) { }

        public virtual void HandleHeal(Player owner, int amount)
        {
            owner.CurrentHealth = Mathf.Clamp(
                owner.CurrentHealth + amount,
                0,
                owner.Data.MaxHealth
            );
            owner.RaiseOnHealthChanged(owner.CurrentHealth / (float)owner.Data.MaxHealth);
        }

        public virtual void HandleDamage(Player owner, int amount)
        {
            if (owner.IsInvincible)
                return;

            owner.IsInvincible = true;
            owner.DamageCooldown = owner.Data.InvincibilityDuration;

            owner.Animator.SetTrigger(AnimatorHashes.Hit);
            owner.OneShotSource.PlayOneShot(owner.HitClip);

            CameraShake.Instance?.Shake();

            owner.CurrentHealth = Mathf.Clamp(
                owner.CurrentHealth - amount,
                0,
                owner.Data.MaxHealth
            );
            owner.RaiseOnHealthChanged(owner.CurrentHealth / (float)owner.Data.MaxHealth);

            if (owner.CurrentHealth == 0)
            {
                owner.RaiseOnDied();
                owner.ChangeState(owner.DeadState);
            }
        }
    }
}
