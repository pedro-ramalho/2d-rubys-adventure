using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    public static class QuestMusic
    {
        public static AudioClip ResolveTrack()
        {
            if (QuestManager.Instance != null)
                foreach (QuestController c in QuestManager.Instance.All)
                    if (c.Phase == QuestPhase.During && c.Data != null && c.Data.BackgroundTrack != null)
                        return c.Data.BackgroundTrack;

            return SceneMusicConfigManager.Instance != null ? SceneMusicConfigManager.Instance.DefaultTrack : null;
        }

        public static void Refresh()
        {
            if (MusicManager.Instance == null) 
                return;

            AudioClip target = ResolveTrack();

            if (target != null) 
                MusicManager.Instance.Play(target);
            else 
                MusicManager.Instance.FadeOutAndStop(5f);
        }

        public static void PlayCompletionStinger(AudioClip stinger)
        {
            if (MusicManager.Instance == null) 
                return;

            AudioClip next = ResolveTrack();

            if (stinger != null)   
                MusicManager.Instance.PlayWithStinger(stinger, next);
            else if (next != null) 
                MusicManager.Instance.Play(next);
            else                   
                MusicManager.Instance.FadeOutAndStop(5f);
        }
    }
}
