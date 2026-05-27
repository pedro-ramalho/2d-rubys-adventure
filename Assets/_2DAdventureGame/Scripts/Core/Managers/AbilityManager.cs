using System;
using UnityEngine;

[Flags]
public enum AbilityFlag
{
    None  = 0,
    Dash  = 1 << 0,
    Shoot = 1 << 1    
}

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    [SerializeField] private AbilityFlag startingAbilities = AbilityFlag.None;

    private AbilityFlag unlocked;

    public bool CanDash => unlocked.HasFlag(AbilityFlag.Dash);
    public bool CanShoot => unlocked.HasFlag(AbilityFlag.Shoot);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log($"[AbilityManager] Duplicate destroyed in scene '{gameObject.scene.name}'");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        unlocked = startingAbilities;
    }

    public void Unlock(AbilityFlag abilities)
    {
        unlocked |= abilities;
        Debug.Log($"[AbilityManager] Unlock called with {abilities}. Now unlocked = {unlocked}");
    }
}
