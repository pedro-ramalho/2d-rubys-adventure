using UnityEngine;

namespace AdventureGame.Core
{
    public abstract class PersistentSingleton<T> : MonoBehaviour
        where T : PersistentSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = (T)this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        protected static void BootstrapIfMissing()
        {
            if (Instance != null)
                return;

            new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}
