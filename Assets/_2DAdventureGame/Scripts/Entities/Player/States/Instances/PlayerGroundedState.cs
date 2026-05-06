using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private Vector2 move;

    private static readonly int NPCMaskHash = LayerMask.GetMask("NPC");

    public override void Update(Player owner)
    {
        move = owner.MoveAction.ReadValue<Vector2>();

        UpdateMoveDirection(owner);
        UpdateAnimator(owner);
        UpdateWalkAudio(owner);
        HandleNPCInteraction(owner);

        if (owner.ShootAction.WasPressedThisFrame())
            Shoot(owner);

        if (owner.DashAction.WasPressedThisFrame() && owner.DashCooldownTimer <= 0f)
            owner.ChangeState(owner.DashingState);
    }

    public override void FixedUpdate(Player owner)
    {
        Vector2 targetVelocity = move * owner.Data.speed;
        float rate = move.magnitude > 0f ? owner.Data.acceleration : owner.Data.deceleration;
        owner.CurrentVelocity = Vector2.MoveTowards(owner.CurrentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        owner.Rigidbody.MovePosition(owner.Rigidbody.position + owner.CurrentVelocity * Time.fixedDeltaTime);
    }

    public override void Exit(Player owner)
    {
        if (owner.AudioSource.clip == owner.WalkClip)
            owner.AudioSource.Stop();
    }

    private void UpdateMoveDirection(Player owner)
    {
        if (!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.y, 0f))
            owner.MoveDirection = move.normalized;
    }

    private void UpdateAnimator(Player owner)
    {
        owner.Animator.SetFloat(Player.LookXHash, owner.MoveDirection.x);
        owner.Animator.SetFloat(Player.LookYHash, owner.MoveDirection.y);
        owner.Animator.SetFloat(Player.SpeedHash, move.magnitude);
    }

    private void UpdateWalkAudio(Player owner)
    {
        if (move.magnitude > 0f)
        {
            if (owner.AudioSource.clip != owner.WalkClip || !owner.AudioSource.isPlaying)
            {
                owner.AudioSource.clip = owner.WalkClip;
                owner.AudioSource.Play();
            }
        }
        else if (owner.AudioSource.clip == owner.WalkClip && owner.AudioSource.isPlaying)
        {
            owner.AudioSource.Stop();
        }
    }

    private void HandleNPCInteraction(Player owner)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            (Vector2)owner.transform.position + Vector2.up * 0.2f,
            owner.MoveDirection,
            1.5f,
            NPCMaskHash
        );

        if (hit.collider != null && hit.collider.TryGetComponent(out NPC npc))
        {
            npc.dialogueBubble.SetActive(true);
            owner.LastNPC = npc;

            if (owner.TalkAction.WasPressedThisFrame())
                UIHandler.Instance.DisplayDialogue();
        }
        else if (owner.LastNPC != null)
        {
            owner.LastNPC.dialogueBubble.SetActive(false);
            owner.LastNPC = null;
        }
    }

    private void Shoot(Player owner)
    {
        owner.Animator.SetTrigger(Player.ShootHash);
        owner.AudioSource.PlayOneShot(owner.LaunchClip);

        GameObject projectileObj = Object.Instantiate(
            owner.ProjectilePrefab,
            owner.Rigidbody.position + Vector2.up * 0.5f,
            Quaternion.identity
        );

        if (projectileObj.TryGetComponent(out Projectile projectile))
            projectile.Launch(owner.MoveDirection, owner.Data.projectileLaunchForce);
    }
}
