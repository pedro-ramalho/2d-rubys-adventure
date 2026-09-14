using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quests
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "CountedQuestController")]
    public class Quest : MonoBehaviour
    {
        [FormerlySerializedAs("data")]
        [SerializeField] private QuestData m_QuestData;

        private readonly HashSet<string> m_ConsumedIds = new();

        public QuestData Data => m_QuestData;
        public QuestPhase Phase { get; private set; } = QuestPhase.Before;

        public int Count => m_ConsumedIds.Count;
        public int Target => m_QuestData != null ? m_QuestData.TargetCount : 0;

        public event Action<Quest> OnPhaseChanged;

        public static event Action<Quest> OnAnyPhaseChanged;

        void Awake()
        {
            if (m_QuestData == null || string.IsNullOrEmpty(m_QuestData.Id))
            {
                Debug.LogError($"[Quest:{name}] QuestData missing or has no id.");
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
            if (Phase != QuestPhase.Before)
                return;

            SetPhase(QuestPhase.During);
        }

        public void MarkComplete()
        {
            if (Phase != QuestPhase.During)
                return;

            SetPhase(QuestPhase.After);
        }

        public bool IsConsumed(string worldId) =>
            !string.IsNullOrEmpty(worldId) && m_ConsumedIds.Contains(worldId);

        public bool CanReport(string worldId) =>
            IsCounted &&
            Phase == QuestPhase.During &&
            !string.IsNullOrEmpty(worldId) &&
            !m_ConsumedIds.Contains(worldId);

        public bool TryReport(string worldId)
        {
            if (!CanReport(worldId))
                return false;

            m_ConsumedIds.Add(worldId);

            if (m_ConsumedIds.Count >= Target)
                MarkComplete();

            return true;
        }

        public QuestSaveData Capture() => new()
        {
            QuestId = m_QuestData.Id,
            Phase = ShouldResetDuring ? QuestPhase.Before : Phase,
            ConsumedIds = IsCounted ? new List<string>(m_ConsumedIds) : null
        };

        public void Restore(QuestSaveData saved)
        {
            m_ConsumedIds.Clear();

            if (saved.ConsumedIds != null)
                foreach (string id in saved.ConsumedIds)
                    if (!string.IsNullOrEmpty(id))
                        m_ConsumedIds.Add(id);

            SetPhase(saved.Phase);
        }

        private bool ShouldResetDuring =>
            m_QuestData != null &&
            m_QuestData.CompletionMode == QuestCompletionMode.External &&
            Phase == QuestPhase.During;

        private bool IsCounted =>
            m_QuestData != null && m_QuestData.CompletionMode == QuestCompletionMode.Counted;

        void SetPhase(QuestPhase next)
        {
            if (Phase == next)
                return;

            Phase = next;

            OnPhaseChanged?.Invoke(this);
            OnAnyPhaseChanged?.Invoke(this);
        }
    }
}
