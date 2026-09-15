using System;
using AdventureGame.Core.Quests;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    [Flags]
    public enum AbilityFlag
    {
        None = 0,
        Dash = 1 << 0,
        Shoot = 1 << 1,
    }

    public class AbilityManager : PersistentSingleton<AbilityManager>
    {
        [SerializeField]
        private AbilityFlag m_StartingAbilities = AbilityFlag.None;

        private AbilityFlag m_UnlockedAbilities;

        public bool CanDash => (m_UnlockedAbilities & AbilityFlag.Dash) != 0;
        public bool CanShoot => (m_UnlockedAbilities & AbilityFlag.Shoot) != 0;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            Quest.OnAnyPhaseChanged += OnQuestPhaseChanged;

            m_UnlockedAbilities = m_StartingAbilities;
        }

        void OnQuestPhaseChanged(Quest quest)
        {
            if (quest.Phase != QuestPhase.During)
                return;

            if (quest.Data == null)
                return;

            Unlock(quest.Data.UnlockOnAccept);
        }

        public void Unlock(AbilityFlag abilities) => m_UnlockedAbilities |= abilities;
    }
}
