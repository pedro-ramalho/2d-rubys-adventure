using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest")]
    public class QuestData : ScriptableObject
    {
        public string Id;
        public string Description;

        public int TargetCount;

        public AbilityFlag UnlockOnAccept = AbilityFlag.None;

        public AudioClip BackgroundTrack;
    }
}
