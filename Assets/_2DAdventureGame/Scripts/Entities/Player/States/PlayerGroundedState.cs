using AdventureGame.Core.Constants;
using AdventureGame.Core.Managers;
using AdventureGame.Core.Scene;
using AdventureGame.UI;
using UnityEngine;

namespace AdventureGame.Entities.Player.States
{
    public class PlayerGroundedState : PlayerState
    {
        private Vector2 move;

        private static readonly int NPCMaskHash = LayerMask.GetMask("NPC");

        public override void Update(Player owner)
        {
            move = owner.MoveAction.ReadValue<Vector2>();

            UpdateMoveDirection(owner);
            UpdateAnimator(owner);
            HandleNPCInteraction(owner);

            if (owner.ShootAction.WasPressedThisFrame() && AbilityManager.Instance != null && AbilityManager.Instance.CanShoot)
            {
                owner.ChangeState(owner.ShootingState);
            
                return;
            }

            if (owner.DashAction.WasPressedThisFrame() && owner.DashCooldownTimer <= 0f && AbilityManager.Instance != null && AbilityManager.Instance.CanDash)
                owner.ChangeState(owner.DashingState);
        }

        public override void FixedUpdate(Player owner)
        {
            Vector2 targetVelocity = move * owner.Data.speed;
            float rate = move.magnitude > 0f ? owner.Data.acceleration : owner.Data.deceleration;
        
            owner.CurrentVelocity = Vector2.MoveTowards(owner.CurrentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
            owner.Rigidbody.MovePosition(owner.Rigidbody.position + owner.CurrentVelocity * Time.fixedDeltaTime);
        }

        private void UpdateMoveDirection(Player owner)
        {
            if (!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.y, 0f))
                owner.MoveDirection = move.normalized;
        }

        private void UpdateAnimator(Player owner)
        {
            owner.Animator.SetFloat(AnimatorHashes.LookX, owner.MoveDirection.x);
            owner.Animator.SetFloat(AnimatorHashes.LookY, owner.MoveDirection.y);
            owner.Animator.SetFloat(AnimatorHashes.Speed, move.magnitude);
        }

        private void HandleNPCInteraction(Player owner)
        {
            if (SceneTransitioner.Instance != null && SceneTransitioner.Instance.IsTransitioning)
            {
                if (InteractPromptPresenter.Instance != null)
                    InteractPromptPresenter.Instance.HideInteractPrompt();
                return;
            }

            RaycastHit2D hit = Physics2D.CircleCast(
                (Vector2)owner.transform.position + Vector2.up * 0.2f,
                0.4f,
                owner.MoveDirection,
                1.5f,
                NPCMaskHash
            );

            NPC.NPC npc = null;
            if (hit.collider != null) hit.collider.TryGetComponent(out npc);

            if (InteractPromptPresenter.Instance != null)
            {
                if (npc != null)
                    InteractPromptPresenter.Instance.ShowInteractPrompt("Press X to talk");
                else
                    InteractPromptPresenter.Instance.HideInteractPrompt();
            }

            if (!owner.TalkAction.WasPressedThisFrame() || npc == null) 
                return;

            if (DialoguePresenter.Instance != null && DialoguePresenter.Instance.IsTyping)
                DialoguePresenter.Instance.Skip();
            else
                npc.Talk();
        }

    }
}
