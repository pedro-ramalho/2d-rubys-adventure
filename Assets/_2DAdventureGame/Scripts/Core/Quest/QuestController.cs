using System;
using UnityEngine;

public abstract class QuestController : MonoBehaviour
{
    [SerializeField] protected QuestData data;

    [Tooltip("Optional stinger played when this quest transitions from During to After.")]
    [SerializeField] private AudioClip completionSfx;

    public QuestData Data => data;
    public QuestPhase Phase { get; protected set; } = QuestPhase.Before;

    public event Action<QuestController> OnPhaseChanged;
    public event Action<QuestController> OnConcluded;
    public event Action<QuestController> OnEpilogueFinished;

    protected virtual void Awake()
    {
        if (data == null || string.IsNullOrEmpty(data.id))
        {
            Debug.LogError($"[QuestController:{name}] QuestData missing or has no id.");
            return;
        }
        if (QuestManager.Instance != null) QuestManager.Instance.Register(this);
    }

    protected virtual void OnDestroy()
    {
        if (QuestManager.Instance != null) QuestManager.Instance.Unregister(this);
    }

    public virtual void Accept()
    {
        if (Phase != QuestPhase.Before) return;
        SetPhase(QuestPhase.During);
        ApplyUnlock();
        QuestMusic.Refresh();
    }

    public virtual void MarkComplete()
    {
        if (Phase != QuestPhase.During) return;
        SetPhase(QuestPhase.After);
        PlayCompletionAudio();
    }

    public void Conclude()
    {
        if (Phase != QuestPhase.After) return;
        OnConcluded?.Invoke(this);
    }

    public void EpilogueFinished()
    {
        if (Phase != QuestPhase.After) return;
        OnEpilogueFinished?.Invoke(this);
    }

    protected void SetPhase(QuestPhase next)
    {
        if (Phase == next) return;
        Phase = next;
        OnPhaseChanged?.Invoke(this);
    }

    protected void ApplyUnlock()
    {
        if (AbilityManager.Instance != null)
            AbilityManager.Instance.Unlock(data.unlockOnAccept);
    }

    void PlayCompletionAudio()
    {
        if (MusicManager.Instance == null) return;

        AudioClip next = QuestMusic.ResolveTrack();

        if (completionSfx != null)
            MusicManager.Instance.PlayWithStinger(completionSfx, next);
        else if (next != null)
            MusicManager.Instance.Play(next);
        else
            MusicManager.Instance.FadeOutAndStop(5f);
    }

    public abstract QuestSaveData Capture();
    public abstract void Restore(QuestSaveData saved);
}
