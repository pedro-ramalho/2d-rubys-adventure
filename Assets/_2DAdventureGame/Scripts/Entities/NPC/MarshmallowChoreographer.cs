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

        void Start() => Quest.OnStateChanged += HandleStateChanged;

        void OnDestroy() => Quest.OnStateChanged -= HandleStateChanged;

        void HandleStateChanged(Quest quest)
        {
            if (quest.Data != m_QuestData)
                return;

            if (quest.State == QuestState.Active)
                m_Marshmallow.WalkToExit();
            else if (quest.State == QuestState.Complete)
                m_Marshmallow.WalkBack();
            else if (quest.State == QuestState.Concluded)
                m_Marshmallow.DisableCollisions();
        }
    }
}
