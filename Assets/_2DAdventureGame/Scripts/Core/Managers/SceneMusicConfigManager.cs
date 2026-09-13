using AdventureGame.Core.Quest;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    public class SceneMusicConfigManager : SceneSingleton<SceneMusicConfigManager>
    {
        [SerializeField] private AudioClip defaultTrack;

        public AudioClip DefaultTrack => defaultTrack;

        void Start() => QuestMusic.Refresh();
    }
}
