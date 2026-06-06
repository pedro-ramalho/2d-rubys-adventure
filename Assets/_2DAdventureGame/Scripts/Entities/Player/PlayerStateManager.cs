public class PlayerStateManager : PersistentSingleton<PlayerStateManager>
{
    public int? StoredHealth { get; private set; }

    public void StoreHealth(int health) => StoredHealth = health;
    public void Clear() => StoredHealth = null;
}
