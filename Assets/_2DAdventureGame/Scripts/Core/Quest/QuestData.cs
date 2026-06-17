using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest")]
public class QuestData : ScriptableObject
{
    public string id;
    public string description;

    [Tooltip("Number of contributors required to complete (e.g. 6 boxes, 4 robots). Ignored by wave-style quests.")]
    public int targetCount;

    [Header("Progression")]
    [Tooltip("Abilities unlocked when this quest is accepted.")]
    public AbilityFlag unlockOnAccept = AbilityFlag.None;

    [Header("Audio")]
    [Tooltip("Background track played while this quest is active. Leave empty to keep the scene's default track.")]
    public AudioClip backgroundTrack;
}
