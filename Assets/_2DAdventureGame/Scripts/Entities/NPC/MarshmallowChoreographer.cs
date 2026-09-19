using AdventureGame.Core.Quests;
using UnityEngine;

namespace AdventureGame.Entities.NPC
{
    [RequireComponent(typeof(Marshmallow))]
    public class MarshmallowChoreographer : MonoBehaviour
    {
        [SerializeField]
        private QuestData m_QuestData;

        private Marshmallow m_Marshmallow;
        private Quest m_Quest;

        void Awake() => m_Marshmallow = GetComponent<Marshmallow>();

        void Start()
        {
            NPC.OnEpilogueEnded += HandleEpilogueEnded;

            if (QuestManager.Instance == null)
                return;

            m_Quest = QuestManager.Instance.Get(m_QuestData);
            if (m_Quest == null)
                return;

            m_Quest.OnPhaseChanged += OnQuestPhaseChanged;
        }

        void OnDestroy()
        {
            NPC.OnEpilogueEnded -= HandleEpilogueEnded;

            if (m_Quest != null)
                m_Quest.OnPhaseChanged -= OnQuestPhaseChanged;
        }

        void OnQuestPhaseChanged(Quest quest)
        {
            if (quest.State == QuestState.Active)
                m_Marshmallow.WalkToExit();
            else if (quest.State == QuestState.Complete)
                m_Marshmallow.WalkBack();
        }

        void HandleEpilogueEnded(QuestData data)
        {
            if (data == m_QuestData)
                m_Marshmallow.DisableCollisions();
        }
    }
}
