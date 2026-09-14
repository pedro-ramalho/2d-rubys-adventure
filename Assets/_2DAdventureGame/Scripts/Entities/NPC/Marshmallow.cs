using System.Collections;
using AdventureGame.Core.Constants;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.NPC
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Marshmallow : NPC
    {
        [FormerlySerializedAs("exitPoint")]
        [SerializeField] private Transform m_ExitPointPosition;

        [FormerlySerializedAs("walkSpeed")]
        [SerializeField] private float m_WalkSpeed = 2f;

        [FormerlySerializedAs("idleFacing")]
        [SerializeField] private Vector2 m_IdleFacing = Vector2.down;

        private Animator m_Animator;
        private Rigidbody2D m_Rigidbody;
        private Collider2D[] m_Colliders;
        private Vector2 m_StartPosition;

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Colliders = GetComponents<Collider2D>();
            
            m_StartPosition = transform.position;

            SetFacing(m_IdleFacing);
        }

        public void WalkToExit()
        {
            if (m_ExitPointPosition != null)
                StartCoroutine(WalkTo(m_ExitPointPosition.position));
        }

        public void WalkBack() => StartCoroutine(WalkTo(m_StartPosition));

        public void DisableCollisions() => SetCollidersEnabled(false);

        IEnumerator WalkTo(Vector2 target)
        {
            SetCollidersEnabled(false);

            Vector2 direction = (target - m_Rigidbody.position).normalized;
            SetFacing(direction);
            m_Animator.SetFloat(AnimatorHashes.Speed, 1f);

            WaitForFixedUpdate wait = new();
            while (Vector2.Distance(m_Rigidbody.position, target) > 0.01f)
            {
                Vector2 next = Vector2.MoveTowards(m_Rigidbody.position, target, m_WalkSpeed * Time.fixedDeltaTime);
                m_Rigidbody.MovePosition(next);

                yield return wait;
            }

            SetFacing(m_IdleFacing);
            m_Animator.SetFloat(AnimatorHashes.Speed, 0f);

            SetCollidersEnabled(true);
        }

        void SetFacing(Vector2 direction)
        {
            m_Animator.SetFloat(AnimatorHashes.LookX, direction.x);
            m_Animator.SetFloat(AnimatorHashes.LookY, direction.y);
        }

        void SetCollidersEnabled(bool value)
        {
            foreach (Collider2D collider in m_Colliders)
                collider.enabled = value;
        }
    }
}
