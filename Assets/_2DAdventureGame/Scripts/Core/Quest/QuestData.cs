using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest")]
public class QuestData : ScriptableObject
{
    public string id;
    public string description;
    public QuestObjective objective;

    [Header("Progression")]
    [Tooltip("Abilities unlocked when this quest is accepted.")]
    public AbilityFlag unlockOnAccept = AbilityFlag.None;

    [Header("Audio")]
    [Tooltip("Background track played while this quest is active. Leave empty to keep the scene's default track.")]
    public AudioClip backgroundTrack;

    [Header("Save Behavior")]
    [Tooltip("If true, an in-progress save of this quest is discarded on reload — the quest, its count, and any consumed world IDs are wiped so the level starts fresh. Use for quests whose progress is meaningless without the dynamic world state (e.g. wave arenas).")]
    public bool resetIfActiveOnReload;
}
