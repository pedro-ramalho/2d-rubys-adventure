using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Quest;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.NPC
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Marshmallow : NPC
    {
        [FormerlySerializedAs("boundQuest")]
        [SerializeField] private QuestData m_BoundQuestData;

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
        private QuestController m_QuestController;

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Colliders = GetComponents<Collider2D>();
            m_StartPosition = transform.position;

            SetFacing(m_IdleFacing);
        }

        void Start()
        {
            if (QuestManager.Instance == null) 
                return;
        
            m_QuestController = QuestManager.Instance.Get(m_BoundQuestData);
            if (m_QuestController == null) 
                return;

            m_QuestController.OnPhaseChanged += HandlePhaseChanged;
            m_QuestController.OnEpilogueFinished += HandleEpilogueFinished;
        }

        void OnDestroy()
        {
            if (m_QuestController != null)
            {
                m_QuestController.OnPhaseChanged -= HandlePhaseChanged;
                m_QuestController.OnEpilogueFinished -= HandleEpilogueFinished;
            }
        }

        void HandlePhaseChanged(QuestController c)
        {
            if (c.Phase == QuestPhase.During) 
                WalkToExit();
            else if (c.Phase == QuestPhase.After) 
                WalkBack();
        }

        void HandleEpilogueFinished(QuestController c) => SetCollidersEnabled(false);

        public void WalkToExit()
        {
            if (m_ExitPointPosition != null) 
                StartCoroutine(WalkTo(m_ExitPointPosition.position));
        }

        public void WalkBack() => StartCoroutine(WalkTo(m_StartPosition));

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
