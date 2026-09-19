using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    public class QuestStateVisibilityGate : MonoBehaviour
    {
        [SerializeField]
        private QuestDefinition m_Quest;

        [SerializeField]
        private QuestState m_ShowFrom = QuestState.Active;

        private readonly List<SpriteRenderer> m_SpriteRenderers = new();
        private readonly List<Collider2D> m_Colliders = new();

        void Awake()
        {
            GetComponentsInChildren(true, m_SpriteRenderers);
            GetComponentsInChildren(true, m_Colliders);

            SetVisible(false);

            Quest.OnStateChanged += HandleStateChanged;
        }

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            Quest quest = QuestManager.Instance.Get(m_Quest);
            if (quest != null && HasReached(quest.State))
                SetVisible(true);
        }

        void OnDestroy() => Quest.OnStateChanged -= HandleStateChanged;

        void HandleStateChanged(Quest quest)
        {
            if (quest.Data != m_Quest)
                return;

            if (HasReached(quest.State))
                SetVisible(true);
        }

        void SetVisible(bool value)
        {
            foreach (SpriteRenderer renderer in m_SpriteRenderers)
                if (renderer != null)
                    renderer.enabled = value;

            foreach (Collider2D collider in m_Colliders)
                if (collider != null)
                    collider.enabled = value;
        }

        bool HasReached(QuestState current) => (int)current >= (int)m_ShowFrom;
    }
}
