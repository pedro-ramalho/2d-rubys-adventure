using AdventureGame.Core.Quest;
using AdventureGame.Entities.Player;
using UnityEngine;

namespace AdventureGame.Environment.Collectibles
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] protected AudioClip collectibleClip;
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
            if (reporter != null && reporter.Quest != null && !reporter.CanReport()) 
                return;

            ApplyEffect(player);

            if (collectibleClip != null)
                player.OneShotSource.PlayOneShot(collectibleClip);

            OnEffectApplied();
            if (reporter != null) 
                reporter.Report();
        }
    }
}
