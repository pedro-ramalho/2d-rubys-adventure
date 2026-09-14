using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest")]
    public class QuestData : ScriptableObject
    {
        public string id;
        public string description;

        public int targetCount;

        public AbilityFlag unlockOnAccept = AbilityFlag.None;

        public AudioClip backgroundTrack;
    }
}
