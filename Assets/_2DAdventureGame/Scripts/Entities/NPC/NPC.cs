using System.Collections.Generic;
using AdventureGame.Core.Dialogue;
using AdventureGame.Core.Quest;
using AdventureGame.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.NPC
{
    public class NPC : MonoBehaviour
    {
        [FormerlySerializedAs("dialogues")]
        [SerializeField] private List<QuestDialogue> m_DialogueLines;

        private DialoguePhase m_CurrentDialoguePhase;
        private QuestDialogue m_CurrentDialogue;
        private int lineIndex;

        public void Talk()
        {
            if (m_CurrentDialoguePhase == null)
            {
                foreach (QuestDialogue dialogue in m_DialogueLines)
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(dialogue.Quest) : null;
                    DialoguePhase phase = dialogue.Pick(controller);
                    if (phase != null)
                    {
                        m_CurrentDialogue = dialogue;
                        m_CurrentDialoguePhase = phase;

                        break;
                    }
                }

                if (m_CurrentDialoguePhase == null) 
                    return;
            
                lineIndex = 0;

                if (m_CurrentDialogue.IsAfterPhase(m_CurrentDialoguePhase))
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(m_CurrentDialogue.Quest) : null;
                    if (controller != null) 
                        controller.Conclude();
                }
            }

            DialoguePresenter.Instance.DisplayDialogueWithLine(m_CurrentDialoguePhase.Lines[lineIndex++], transform);

            if (lineIndex >= m_CurrentDialoguePhase.Lines.Count)
            {
                if (m_CurrentDialoguePhase.QuestToGrantAfter != null)
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(m_CurrentDialoguePhase.QuestToGrantAfter) : null;
                    if (controller != null) 
                        controller.Accept();
                }

                if (m_CurrentDialogue.IsAfterPhase(m_CurrentDialoguePhase))
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(m_CurrentDialogue.Quest) : null;
                    if (controller != null) 
                        controller.EpilogueFinished();
                }

                m_CurrentDialoguePhase.OnExhausted?.Invoke();
                m_CurrentDialoguePhase = null;
                m_CurrentDialogue = null;
            }
        }
    }
}
