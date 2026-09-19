using System;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    public class Quest : MonoBehaviour
    {
        [SerializeField]
        private QuestDefinition m_QuestData;

        private int m_Count;

        public QuestDefinition Data => m_QuestData;
        public QuestState State { get; private set; } = QuestState.Inactive;

        public int Count => m_Count;
        public int Target => m_QuestData != null ? m_QuestData.TargetCount : 0;

        public bool CanReport => IsCounted && State == QuestState.Active;

        public static event Action<Quest> OnStateChanged;

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

        public void Conclude()
        {
            if (State != QuestState.Complete)
                return;

            SetState(QuestState.Concluded);
        }

        public bool TryReport()
        {
            if (!CanReport)
                return false;

            m_Count++;

            if (m_Count >= Target)
                MarkComplete();

            return true;
        }

        public QuestSaveData Capture() =>
            new()
            {
                QuestId = m_QuestData.Id,
                State = State == QuestState.Active ? QuestState.Inactive : State,
            };

        public void Restore(QuestSaveData saved) => SetState(saved.State);

        private bool IsCounted => Target > 0;

        void SetState(QuestState next)
        {
            if (State == next)
                return;

            State = next;

            OnStateChanged?.Invoke(this);
        }
    }
}
