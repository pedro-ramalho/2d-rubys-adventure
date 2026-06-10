using UnityEngine;

public class PlayerShootingState : PlayerState
{
    private float timer;

    public override void Enter(Player owner)
    {
        timer = owner.Data.shootDuration;
        owner.CurrentVelocity = Vector2.zero;

        owner.Animator.SetTrigger(AnimatorHashes.Launch);
        owner.OneShotSource.PlayOneShot(owner.LaunchClip);

        GameObject projectileObj = Object.Instantiate(
            owner.ProjectilePrefab,
            owner.Rigidbody.position + Vector2.up * 0.5f,
            Quaternion.identity
        );

        if (projectileObj.TryGetComponent(out Projectile projectile))
            projectile.Launch(owner.MoveDirection, owner.Data.projectileLaunchForce);
    }

    public override void Update(Player owner)
    {
        timer -= Time.deltaTime;

        if (owner.DashAction.WasPressedThisFrame() && owner.DashCooldownTimer <= 0f)
        {
            owner.ChangeState(owner.DashingState);
            return;
        }

        if (timer <= 0f)
            owner.ChangeState(owner.GroundedState);
    }
}
