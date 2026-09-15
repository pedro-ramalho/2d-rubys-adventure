using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest")]
    public class QuestData : ScriptableObject
    {
        public string Id;

        public string Description;

        public QuestCompletionMode CompletionMode;

        public int TargetCount;

        public AbilityFlag UnlockOnAccept = AbilityFlag.None;

        public AudioClip BackgroundTrack;

        public AudioClip CompletionStinger;
    }
}
