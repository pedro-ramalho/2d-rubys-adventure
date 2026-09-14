using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quest
{
    public class QuestBinding : MonoBehaviour
    {
        [FormerlySerializedAs("boundQuest")]
        [SerializeField] private QuestData m_BoundQuestData;

        [FormerlySerializedAs("activateOnQuestAccept")]
        [SerializeField] private bool m_ActivateOnQuestAccept;

        [FormerlySerializedAs("enableTriggerOnQuestAccept")]
        [SerializeField] private bool m_EnableTriggerOnQuestAccept;

        [FormerlySerializedAs("deactivateOnComplete")]
        [SerializeField] private bool m_DeactivateOnComplete = true;

        [FormerlySerializedAs("interactables")]
        [SerializeField] private Behaviour[] m_Interactables;

        private readonly List<SpriteRenderer> m_SpriteRenderers = new();
        private readonly List<Collider2D> m_Colliders = new();
        private QuestController m_QuestController;

        void Awake()
        {
            if (m_ActivateOnQuestAccept || m_EnableTriggerOnQuestAccept)
                GetComponentsInChildren(true, m_Colliders);

            if (m_ActivateOnQuestAccept)
                GetComponentsInChildren(true, m_SpriteRenderers);

            SetInteractable(false);
        }

        void Start()
        {
            if (QuestManager.Instance == null) 
                return;

            m_QuestController = QuestManager.Instance.Get(m_BoundQuestData);
            if (m_QuestController == null) 
                return;

            QuestReporter reporter = GetComponent<QuestReporter>();
            if (reporter != null && reporter.IsConsumed())
            {
                gameObject.SetActive(false);

                return;
            }

            m_QuestController.OnPhaseChanged += HandlePhaseChanged;
        
            SetInteractable(m_QuestController.Phase == QuestPhase.During);
        }

        void OnDestroy()
        {
            if (m_QuestController != null)
                m_QuestController.OnPhaseChanged -= HandlePhaseChanged;
        }

        void HandlePhaseChanged(QuestController c)
        {
            if (c.Phase == QuestPhase.During)
            {
                SetInteractable(true);
            }
            else if (c.Phase == QuestPhase.After && m_DeactivateOnComplete)
            {
                SetInteractable(false);
            }
        }

        void SetInteractable(bool value)
        {
            foreach (Behaviour b in m_Interactables)
                if (b != null) b.enabled = value;

            if (m_ActivateOnQuestAccept)
            {
                foreach (SpriteRenderer r in m_SpriteRenderers)
                    if (r != null) 
                        r.enabled = value;

                foreach (Collider2D c in m_Colliders)
                    if (c != null) 
                        c.enabled = value;
            }

            if (m_EnableTriggerOnQuestAccept)
            {
                foreach (Collider2D c in m_Colliders)
                    if (c != null) 
                        c.isTrigger = value;
            }
        }
    }
}
