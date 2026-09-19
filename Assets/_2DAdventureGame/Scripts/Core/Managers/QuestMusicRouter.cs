using AdventureGame.Core.Quests;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Managers
{
    public class QuestMusicRouter : SceneSingleton<QuestMusicRouter>
    {
        [FormerlySerializedAs("defaultTrack")]
        [SerializeField]
        private AudioClip m_DefaultTrack;
        public AudioClip DefaultTrack => m_DefaultTrack;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            Quest.OnStateChanged += HandleStateChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Quest.OnStateChanged -= HandleStateChanged;
        }

        void Start() => PlayResolvedTrack();

        void HandleStateChanged(Quest quest)
        {
            if (MusicManager.Instance == null)
                return;

            if (quest.State == QuestState.Active)
            {
                PlayResolvedTrack();

                return;
            }

            if (quest.State == QuestState.Complete)
            {
                AudioClip stinger = quest.Data != null ? quest.Data.CompletionStinger : null;
                AudioClip next = ResolveTrack();

                if (stinger != null)
                    MusicManager.Instance.PlayWithStinger(stinger, next);
                else if (next != null)
                    MusicManager.Instance.Play(next);
                else
                    MusicManager.Instance.FadeOutAndStop(5f);
            }
        }

        void PlayResolvedTrack()
        {
            if (MusicManager.Instance == null)
                return;

            AudioClip target = ResolveTrack();

            if (target != null)
                MusicManager.Instance.Play(target);
            else
                MusicManager.Instance.FadeOutAndStop(5f);
        }

        AudioClip ResolveTrack()
        {
            if (QuestManager.Instance == null)
                return m_DefaultTrack;

            foreach (Quest quest in QuestManager.Instance.All)
                if (
                    quest.State == QuestState.Active
                    && quest.Data != null
                    && quest.Data.BackgroundTrack != null
                )
                    return quest.Data.BackgroundTrack;

            return m_DefaultTrack;
        }
    }
}
