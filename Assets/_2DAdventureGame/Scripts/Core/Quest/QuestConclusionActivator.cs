using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quest
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "PhaseGateActivator")]
    public class QuestConclusionActivator : MonoBehaviour
    {
        [FormerlySerializedAs("m_QuestData")]
        [SerializeField] private QuestData m_Quest;

        [FormerlySerializedAs("m_TargetGameObject")]
        [SerializeField] private GameObject m_Target;

        private QuestController m_Controller;

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            m_Controller = QuestManager.Instance.Get(m_Quest);
            if (m_Controller == null)
                return;

            if (m_Controller.Phase == QuestPhase.After)
            {
                ActivateSilent();

                return;
            }

            m_Controller.OnConcluded += OnQuestConcluded;
        }

        void OnDestroy()
        {
            if (m_Controller != null)
                m_Controller.OnConcluded -= OnQuestConcluded;
        }

        void OnQuestConcluded(QuestController _)
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
