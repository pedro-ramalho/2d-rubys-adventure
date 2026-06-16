public class WaveQuestController : QuestController
{
    public override QuestSaveData Capture() => new QuestSaveData
    {
        questId = data.id,
        phase = Phase == QuestPhase.During ? QuestPhase.Before : Phase,
        consumedIds = null
    };

    public override void Restore(QuestSaveData saved)
    {
        SetPhase(saved.phase);

        if (saved.phase != QuestPhase.Before)
            ApplyUnlock();
    }
}
