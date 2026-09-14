using System;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    [Flags]
    public enum AbilityFlag
    {
        None  = 0,
        Dash  = 1 << 0,
        Shoot = 1 << 1    
    }

    public class AbilityManager : PersistentSingleton<AbilityManager>
    {
        [SerializeField] private AbilityFlag startingAbilities = AbilityFlag.None;

        private AbilityFlag unlocked;

        public bool CanDash  => (unlocked & AbilityFlag.Dash)  != 0;
        public bool CanShoot => (unlocked & AbilityFlag.Shoot) != 0;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            unlocked = startingAbilities;
        }

        public void Unlock(AbilityFlag abilities) => unlocked |= abilities;
    }
}