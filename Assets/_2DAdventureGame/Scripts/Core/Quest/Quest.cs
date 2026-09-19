using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    public class Quest : MonoBehaviour
    {
        [SerializeField]
        private QuestDefinition m_QuestData;

        private readonly HashSet<string> m_ConsumedIds = new();

        public QuestDefinition Data => m_QuestData;
        public QuestState State { get; private set; } = QuestState.Inactive;

        public int Count => m_ConsumedIds.Count;
        public int Target => m_QuestData != null ? m_QuestData.TargetCount : 0;

        public event Action<Quest> OnPhaseChanged;

        public static event Action<Quest> OnAnyPhaseChanged;

        void Awake()
        {
            if (m_QuestData == null || string.IsNullOrEmpty(m_QuestData.Id))
            {
                Debug.LogError($"[Quest:{name}] QuestDefinition missing or has no id.");
                return;
            }

            if (QuestManager.Instance != null)
                QuestManager.Instance.Register(this);
        }

        void OnDestroy()
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.Unregister(this);
        }

        public void Accept()
        {
            if (State != QuestState.Inactive)
                return;

            SetState(QuestState.Active);
        }

        public void MarkComplete()
        {
            if (State != QuestState.Active)
                return;

            SetState(QuestState.Complete);
        }

        public bool IsConsumed(string worldId) =>
            !string.IsNullOrEmpty(worldId) && m_ConsumedIds.Contains(worldId);

        public bool CanReport(string worldId) =>
            IsCounted
            && State == QuestState.Active
            && !string.IsNullOrEmpty(worldId)
            && !m_ConsumedIds.Contains(worldId);

        public bool TryReport(string worldId)
        {
            if (!CanReport(worldId))
                return false;

            m_ConsumedIds.Add(worldId);

            if (m_ConsumedIds.Count >= Target)
                MarkComplete();

            return true;
        }

        public QuestSaveData Capture() =>
            new()
            {
                QuestId = m_QuestData.Id,
                State = ShouldResetActive ? QuestState.Inactive : State,
                ConsumedIds = IsCounted ? new List<string>(m_ConsumedIds) : null,
            };

        public void Restore(QuestSaveData saved)
        {
            m_ConsumedIds.Clear();

            if (saved.ConsumedIds != null)
                foreach (string id in saved.ConsumedIds)
                    if (!string.IsNullOrEmpty(id))
                        m_ConsumedIds.Add(id);

            SetState(saved.State);
        }

        private bool ShouldResetActive =>
            m_QuestData != null
            && m_QuestData.CompletionMode == QuestCompletionMode.External
            && State == QuestState.Active;

        private bool IsCounted =>
            m_QuestData != null && m_QuestData.CompletionMode == QuestCompletionMode.Counted;

        void SetState(QuestState next)
        {
            if (State == next)
                return;

            State = next;

            OnPhaseChanged?.Invoke(this);
            OnAnyPhaseChanged?.Invoke(this);
        }
    }
}
