using AdventureGame.Core.Quest;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Managers
{
    public class SceneMusicConfigManager : SceneSingleton<SceneMusicConfigManager>
    {
        [FormerlySerializedAs("defaultTrack")]
        [SerializeField] private AudioClip m_DefaultTrack;

        public AudioClip DefaultTrack => m_DefaultTrack;

        void Start() => QuestMusic.Refresh();
    }
}
