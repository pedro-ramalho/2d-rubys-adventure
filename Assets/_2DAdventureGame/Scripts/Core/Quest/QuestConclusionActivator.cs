using AdventureGame.Entities.NPC;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quests
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "PhaseGateActivator")]
    public class QuestConclusionActivator : MonoBehaviour
    {
        [FormerlySerializedAs("m_QuestData")]
        [SerializeField]
        private QuestData m_Quest;

        [FormerlySerializedAs("m_TargetGameObject")]
        [SerializeField]
        private GameObject m_Target;

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            Quest quest = QuestManager.Instance.Get(m_Quest);
            if (quest != null && quest.Phase == QuestPhase.After)
            {
                ActivateSilent();
                return;
            }

            NPC.OnEpilogueStarted += HandleEpilogueStarted;
        }

        void OnDestroy()
        {
            NPC.OnEpilogueStarted -= HandleEpilogueStarted;
        }

        void HandleEpilogueStarted(QuestData data)
        {
            if (data != m_Quest)
                return;

            if (m_Target != null)
                m_Target.SetActive(true);
        }

        void ActivateSilent()
        {
            if (m_Target == null)
                return;

            foreach (AudioSource source in m_Target.GetComponentsInChildren<AudioSource>(true))
                source.playOnAwake = false;

            m_Target.SetActive(true);
        }
    }
}
