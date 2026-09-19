using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    [CreateAssetMenu(fileName = "QuestDefinition", menuName = "Game/Quest")]
    public class QuestDefinition : ScriptableObject
    {
        public string Id;

        public string Description;

        public int TargetCount;

        public AbilityFlag UnlockOnAccept = AbilityFlag.None;

        public AudioClip BackgroundTrack;

        public AudioClip CompletionStinger;
    }
}
