using AdventureGame.Core.Quests;
using UnityEngine;

namespace AdventureGame.Entities.NPC
{
    [RequireComponent(typeof(Marshmallow))]
    public class MarshmallowCoreagrapher : MonoBehaviour
    {
        [SerializeField] private QuestData m_QuestData;

        private Marshmallow m_Marshmallow;
        private Quest m_Quest;

        void Awake() => m_Marshmallow = GetComponent<Marshmallow>();

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            m_Quest = QuestManager.Instance.Get(m_QuestData);
            if (m_Quest == null)
                return;

            m_Quest.OnPhaseChanged += OnQuestPhaseChanged;
            m_Quest.OnEpilogueFinished += OnQuestEpilogueFinished;
        }

        void OnDestroy()
        {
            if (m_Quest != null)
            {
                m_Quest.OnPhaseChanged -= OnQuestPhaseChanged;
                m_Quest.OnEpilogueFinished -= OnQuestEpilogueFinished;
            }
        }

        void OnQuestPhaseChanged(Quest controller)
        {
            if (controller.Phase == QuestPhase.During)
                m_Marshmallow.WalkToExit();
            else if (controller.Phase == QuestPhase.After)
                m_Marshmallow.WalkBack();
        }

        void OnQuestEpilogueFinished(Quest _) => m_Marshmallow.DisableCollisions();
    }
}
