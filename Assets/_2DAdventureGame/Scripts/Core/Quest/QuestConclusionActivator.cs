using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quests
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "PhaseGateActivator")]
    public class QuestConclusionActivator : MonoBehaviour
    {
        [FormerlySerializedAs("m_QuestData")]
        [SerializeField] private QuestData m_Quest;

        [FormerlySerializedAs("m_TargetGameObject")]
        [SerializeField] private GameObject m_Target;

        private Quest m_ActiveQuest;

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            m_ActiveQuest = QuestManager.Instance.Get(m_Quest);
            if (m_ActiveQuest == null)
                return;

            if (m_ActiveQuest.Phase == QuestPhase.After)
            {
                ActivateSilent();

                return;
            }

            m_ActiveQuest.OnConcluded += OnQuestConcluded;
        }

        void OnDestroy()
        {
            if (m_ActiveQuest != null)
                m_ActiveQuest.OnConcluded -= OnQuestConcluded;
        }

        void OnQuestConcluded(Quest _)
        {
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
