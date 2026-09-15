using AdventureGame.Core.Quests;
using AdventureGame.Entities.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Environment.Collectibles
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField]
        protected AudioClip m_CollectibleClip;

        protected virtual void ApplyEffect(Player player) { }

        protected virtual void OnEffectApplied() => Destroy(gameObject);

        void Start()
        {
            QuestReporter reporter = GetComponent<QuestReporter>();
            if (reporter != null && reporter.IsConsumed())
                gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player player))
                return;

            QuestReporter reporter = GetComponent<QuestReporter>();
            if (reporter != null && reporter.Data != null && !reporter.CanReport())
                return;

            ApplyEffect(player);

            if (m_CollectibleClip != null)
                player.OneShotSource.PlayOneShot(m_CollectibleClip);

            OnEffectApplied();
            if (reporter != null)
                reporter.Report();
        }
    }
}
