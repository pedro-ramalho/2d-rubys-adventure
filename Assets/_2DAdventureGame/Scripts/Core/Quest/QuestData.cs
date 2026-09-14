using AdventureGame.Core.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quest
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest")]
    public class QuestData : ScriptableObject
    {
        [FormerlySerializedAs("id")]
        public string Id;

        [FormerlySerializedAs("description")]
        public string Description;

        [FormerlySerializedAs("targetCount")]
        public int TargetCount;

        [FormerlySerializedAs("unlockOnAccept")]
        public AbilityFlag UnlockOnAccept = AbilityFlag.None;

        [FormerlySerializedAs("backgroundTrack")]
        public AudioClip BackgroundTrack;

        public AudioClip CompletionStinger;
    }
}
