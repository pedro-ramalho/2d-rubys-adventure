using System;
using System.Collections.Generic;
using AdventureGame.Core.Dialogue;
using AdventureGame.Core.Quests;
using AdventureGame.UI;
using UnityEngine;

namespace AdventureGame.Entities.NPC
{
    public class NPC : MonoBehaviour
    {
        [SerializeField]
        private List<QuestDialogue> m_DialogueLines;

        private QuestDialogue m_CurrentDialogue;
        private DialoguePhase m_CurrentPhase;
        private int m_LineIndex;

        public static event Action<QuestDefinition> OnEpilogueStarted;
        public static event Action<QuestDefinition> OnEpilogueEnded;

        public void Talk()
        {
            if (m_CurrentPhase == null && !TryPickPhase())
                return;

            DialoguePresenter.Instance.DisplayDialogueWithLine(
                m_CurrentPhase.Lines[m_LineIndex++],
                transform
            );

            if (m_LineIndex >= m_CurrentPhase.Lines.Count)
                FinishPhase();
        }

        bool TryPickPhase()
        {
            foreach (QuestDialogue dialogue in m_DialogueLines)
            {
                Quest quest =
                    QuestManager.Instance != null
                        ? QuestManager.Instance.Get(dialogue.Quest)
                        : null;

                DialoguePhase phase = dialogue.Pick(quest);
                if (phase == null)
                    continue;

                m_CurrentDialogue = dialogue;
                m_CurrentPhase = phase;
                m_LineIndex = 0;

                if (dialogue.IsEpilogue(phase))
                    OnEpilogueStarted?.Invoke(dialogue.Quest);

                return true;
            }

            return false;
        }

        void FinishPhase()
        {
            QuestDialogue dialogue = m_CurrentDialogue;
            DialoguePhase phase = m_CurrentPhase;

            m_CurrentDialogue = null;
            m_CurrentPhase = null;

            if (phase.QuestToGrantAfter != null && QuestManager.Instance != null)
            {
                Quest granted = QuestManager.Instance.Get(phase.QuestToGrantAfter);
                if (granted != null)
                    granted.Accept();
            }

            if (dialogue.IsEpilogue(phase))
            {
                OnEpilogueEnded?.Invoke(dialogue.Quest);

                Quest quest =
                    QuestManager.Instance != null
                        ? QuestManager.Instance.Get(dialogue.Quest)
                        : null;
                if (quest != null)
                    quest.Conclude();
            }

            phase.OnExhausted?.Invoke();
        }
    }
}
