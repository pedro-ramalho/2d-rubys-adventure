using AdventureGame.Core.Quest;
using UnityEngine;

namespace AdventureGame.Entities.NPC
{
    [RequireComponent(typeof(Marshmallow))]
    public class MarshmallowCoreagrapher : MonoBehaviour
    {
        [SerializeField] private QuestData m_QuestData;

        private Marshmallow m_Marshmallow;
        private QuestController m_QuestController;

        void Awake() => m_Marshmallow = GetComponent<Marshmallow>();

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            m_QuestController = QuestManager.Instance.Get(m_QuestData);
            if (m_QuestController == null)
                return;

            m_QuestController.OnPhaseChanged += OnQuestPhaseChanged;
            m_QuestController.OnEpilogueFinished += OnQuestEpilogueFinished;
        }

        void OnDestroy()
        {
            if (m_QuestController != null)
            {
                m_QuestController.OnPhaseChanged -= OnQuestPhaseChanged;
                m_QuestController.OnEpilogueFinished -= OnQuestEpilogueFinished;
            }
        }

        void OnQuestPhaseChanged(QuestController controller)
        {
            if (controller.Phase == QuestPhase.During)
                m_Marshmallow.WalkToExit();
            else if (controller.Phase == QuestPhase.After)
                m_Marshmallow.WalkBack();
        }

        void OnQuestEpilogueFinished(QuestController _) => m_Marshmallow.DisableCollisions();
    }
}
