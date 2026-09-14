using System;
using System.Collections.Generic;
using AdventureGame.Core.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quest
{
    public abstract class QuestController : MonoBehaviour
    {
        [FormerlySerializedAs("data")]
        [SerializeField] protected QuestData m_QuestData;

        [FormerlySerializedAs("completionSfx")]
        [SerializeField] private AudioClip m_CompletionSfx;

        public QuestData Data => m_QuestData;
        public QuestPhase Phase { get; protected set; } = QuestPhase.Before;

        public event Action<QuestController> OnPhaseChanged;
        public event Action<QuestController> OnConcluded;
        public event Action<QuestController> OnEpilogueFinished;

        protected virtual void Awake()
        {
            if (m_QuestData == null || string.IsNullOrEmpty(m_QuestData.Id))
            {
                Debug.LogError($"[QuestController:{name}] QuestData missing or has no id.");
            
                return;
            }

            if (QuestManager.Instance != null) 
                QuestManager.Instance.Register(this);
        }

        protected virtual void OnDestroy()
        {
            if (QuestManager.Instance != null) 
                QuestManager.Instance.Unregister(this);
        }

        public virtual void Accept()
        {
            if (Phase != QuestPhase.Before) 
                return;

            SetPhase(QuestPhase.During);
            ApplyUnlock();
        
            QuestMusic.Refresh();
        }

        public virtual void MarkComplete()
        {
            if (Phase != QuestPhase.During) 
                return;
        
            SetPhase(QuestPhase.After);
        
            QuestMusic.PlayCompletionStinger(m_CompletionSfx);
        }

        public void Conclude()
        {
            if (Phase != QuestPhase.After) 
                return;
        
            OnConcluded?.Invoke(this);
        }

        public void EpilogueFinished()
        {
            if (Phase != QuestPhase.After) 
                return;
        
            OnEpilogueFinished?.Invoke(this);
        }

        public QuestSaveData Capture() => new QuestSaveData
        {
            QuestId = m_QuestData.Id,
            Phase = CapturePhase(),
            ConsumedIds = CaptureConsumed()
        };

        public void Restore(QuestSaveData saved)
        {
            RestoreData(saved);
            SetPhase(saved.Phase);

            if (saved.Phase != QuestPhase.Before) 
                ApplyUnlock();
        }

        protected virtual QuestPhase CapturePhase() => Phase;
        protected virtual List<string> CaptureConsumed() => null;
        protected virtual void RestoreData(QuestSaveData saved) { }

        protected void SetPhase(QuestPhase next)
        {
            if (Phase == next) 
                return;
        
            Phase = next;
            OnPhaseChanged?.Invoke(this);
        }

        protected void ApplyUnlock()
        {
            if (AbilityManager.Instance != null)
                AbilityManager.Instance.Unlock(m_QuestData.UnlockOnAccept);
        }
    }
}
