namespace AdventureGame.Core.Quest
{
    public class WaveQuestController : QuestController
    {
        protected override QuestPhase CapturePhase() =>
            Phase == QuestPhase.During ? QuestPhase.Before : Phase;
    }
}
