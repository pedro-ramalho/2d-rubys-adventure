using System;
using UnityEngine;

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

    public bool CanDash => unlocked.HasFlag(AbilityFlag.Dash);
    public bool CanShoot => unlocked.HasFlag(AbilityFlag.Shoot);

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        unlocked = startingAbilities;
    }

    public void Unlock(AbilityFlag abilities)
    {
        unlocked |= abilities;
    }
}
