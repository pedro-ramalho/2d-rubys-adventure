using UnityEngine;

public class PlayerDashingState : PlayerState
{
    private float dashTimer;
    private float afterimageTimer;
    private Vector2 dashDirection;

    public override void Enter(Player owner)
    {
        dashTimer = owner.Data.dashDuration;
        dashDirection = owner.MoveDirection;

        owner.DashCooldownTimer = owner.Data.dashCooldown;
        owner.IsInvincible = true;
        owner.DamageCooldown = owner.Data.dashDuration;
        owner.Animator.SetFloat(AnimatorHashes.Speed, 1f);
        owner.OneShotSource.PlayOneShot(owner.DashClip);
        owner.CurrentVelocity = Vector2.zero;
    }

    public override void Update(Player owner)
    {
        if (dashTimer <= 0)
        {
            owner.ChangeState(owner.GroundedState);
            
            return;
        }

        if (afterimageTimer <= 0)
            SpawnAfterimages(owner);

        dashTimer -= Time.deltaTime;
        afterimageTimer -= Time.deltaTime;
    }

    public override void FixedUpdate(Player owner)
    {
        Vector2 offset = dashDirection * owner.Data.dashSpeed * Time.fixedDeltaTime;

        owner.Rigidbody.MovePosition(owner.Rigidbody.position + offset);
    }

    private void SpawnAfterimages(Player owner)
    {
        GameObject ghost = Object.Instantiate(
            owner.AfterimagePrefab,
            owner.transform.position,
            owner.transform.rotation
        );

        if (ghost.TryGetComponent(out DashAfterimage afterimage))
        {
            afterimage.Initialize(
                owner.SpriteRenderer.sprite,
                owner.transform.localScale,
                owner.SpriteRenderer.flipX,
                owner.Data.afterimageColor,
                owner.Data.afterimageLingerDuration
            );
        }

        afterimageTimer = owner.Data.afterimageInterval;
    }
}
