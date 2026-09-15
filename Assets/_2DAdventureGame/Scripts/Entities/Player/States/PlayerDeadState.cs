using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Scene;
using AdventureGame.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdventureGame.Entities.Player.States
{
    public class PlayerDeadState : PlayerState
    {
        private const float k_ReloadDelay = 1.5f;

        public override void Enter(Player owner)
        {
            owner.CurrentVelocity = Vector2.zero;
            owner.Animator.SetFloat(AnimatorHashes.Speed, 0f);

            Time.timeScale = 0f;

            if (EndScreenPresenter.Instance != null)
                EndScreenPresenter.Instance.DisplayLoseScreen();
            owner.StartCoroutine(ReloadAfterDelay());
        }

        private IEnumerator ReloadAfterDelay()
        {
            yield return new WaitForSecondsRealtime(k_ReloadDelay);
            Time.timeScale = 1f;

            string currentScene = SceneManager.GetActiveScene().name;
            if (SceneTransitioner.Instance != null)
                SceneTransitioner.Instance.LoadSceneWithCrossfade(
                    currentScene,
                    0f,
                    writeSave: false
                );
            else
                SceneManager.LoadScene(currentScene);
        }

        public override void HandleHeal(Player owner, int amount) { }

        public override void HandleDamage(Player owner, int amount) { }
    }
}
