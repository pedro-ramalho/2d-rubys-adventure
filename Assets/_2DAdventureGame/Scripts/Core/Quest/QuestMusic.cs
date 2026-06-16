using UnityEngine;

public static class QuestMusic
{
    public static AudioClip ResolveTrack()
    {
        if (QuestManager.Instance != null)
            foreach (QuestController c in QuestManager.Instance.All)
                if (c.Phase == QuestPhase.During && c.Data != null && c.Data.backgroundTrack != null)
                    return c.Data.backgroundTrack;

        return SceneMusicConfigManager.Instance != null ? SceneMusicConfigManager.Instance.DefaultTrack : null;
    }

    public static void Refresh()
    {
        if (MusicManager.Instance == null) return;

        AudioClip target = ResolveTrack();
        if (target != null) MusicManager.Instance.Play(target);
        else MusicManager.Instance.FadeOutAndStop(5f);
    }
}
