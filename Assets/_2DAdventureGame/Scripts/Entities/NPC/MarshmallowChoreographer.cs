using AdventureGame.Core.Quests;
using UnityEngine;

namespace AdventureGame.Entities.NPC
{
    [RequireComponent(typeof(Marshmallow))]
    public class MarshmallowChoreographer : MonoBehaviour
    {
        [SerializeField]
        private QuestDefinition m_QuestData;

        private Marshmallow m_Marshmallow;

        void Awake() => m_Marshmallow = GetComponent<Marshmallow>();

        void Start()
        {
            NPC.OnEpilogueEnded += HandleEpilogueEnded;

            Quest.OnStateChanged += HandleStateChanged;
        }

        void OnDestroy()
        {
            NPC.OnEpilogueEnded -= HandleEpilogueEnded;

            Quest.OnStateChanged -= HandleStateChanged;
        }

        void HandleStateChanged(Quest quest)
        {
            if (quest.Data != m_QuestData)
                return;

            if (quest.State == QuestState.Active)
                m_Marshmallow.WalkToExit();
            else if (quest.State == QuestState.Complete)
                m_Marshmallow.WalkBack();
        }

        void HandleEpilogueEnded(QuestDefinition data)
        {
            if (data == m_QuestData)
                m_Marshmallow.DisableCollisions();
        }
    }
}
