using AdventureGame.Core.Quest;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Managers
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "SceneMusicConfigManager")]
    public class QuestMusicRouter : SceneSingleton<QuestMusicRouter>
    {
        [FormerlySerializedAs("defaultTrack")]
        [SerializeField] private AudioClip m_DefaultTrack;
        public AudioClip DefaultTrack => m_DefaultTrack;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            QuestController.OnAnyPhaseChanged += OnQuestPhaseChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            QuestController.OnAnyPhaseChanged -= OnQuestPhaseChanged;
        }

        void Start() => PlayResolvedTrack();

        void OnQuestPhaseChanged(QuestController quest)
        {
            if (MusicManager.Instance == null)
                return;

            if (quest.Phase == QuestPhase.During)
            {
                PlayResolvedTrack();

                return;
            }
            
            if (quest.Phase == QuestPhase.After)
            {
                AudioClip stinger = quest.Data != null
                    ? quest.Data.CompletionStinger
                    : null;
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

            foreach (QuestController quest in QuestManager.Instance.All)
                if (quest.Phase == QuestPhase.During && quest.Data != null && quest.Data.BackgroundTrack != null)
                    return quest.Data.BackgroundTrack;

            return m_DefaultTrack;
        }
    }
}
