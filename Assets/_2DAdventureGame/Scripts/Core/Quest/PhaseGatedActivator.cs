using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quest
{
    public class PhaseGatedActivator : MonoBehaviour
    {
        [FormerlySerializedAs("quest")]
        [SerializeField] private QuestData m_QuestData;

        [FormerlySerializedAs("target")]
        [SerializeField] private GameObject m_TargetGameObject;

        [FormerlySerializedAs("activateOnConclude")]
        [SerializeField] private bool m_ActivateOnConclude = true;

        private QuestController m_QuestController;

        void Start()
        {
            m_QuestController = QuestManager.Instance != null ? QuestManager.Instance.Get(m_QuestData) : null;
            if (m_QuestController == null) 
                return;

            if (m_QuestController.Phase == QuestPhase.After)
            {
                ActivateSilent();

                return;
            }

            if (m_ActivateOnConclude)
                m_QuestController.OnConcluded += HandleConcluded;
        }

        void OnDestroy()
        {
            if (m_QuestController != null)
                m_QuestController.OnConcluded -= HandleConcluded;
        }

        void HandleConcluded(QuestController _)
        {
            if (m_TargetGameObject != null) 
                m_TargetGameObject.SetActive(true);
        }

        void ActivateSilent()
        {
            if (m_TargetGameObject == null) 
                return;
        
            foreach (AudioSource source in m_TargetGameObject.GetComponentsInChildren<AudioSource>(true))
                source.playOnAwake = false;
        
            m_TargetGameObject.SetActive(true);
        }
    }
}
