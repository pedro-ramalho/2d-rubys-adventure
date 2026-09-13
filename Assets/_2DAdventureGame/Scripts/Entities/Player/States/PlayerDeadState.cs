using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Scene;
using AdventureGame.Core.Wave;
using AdventureGame.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdventureGame.Entities.Player.States
{
    public class PlayerDeadState : PlayerState
    {
        private const float ReloadDelay = 1.5f;

        public override void Enter(Player owner)
        {
            owner.CurrentVelocity = Vector2.zero;
            owner.Animator.SetFloat(AnimatorHashes.Speed, 0f);

            HaltCombat();

            Time.timeScale = 0f;

            if (UIHandler.Instance != null) UIHandler.Instance.DisplayLoseScreen();
            owner.StartCoroutine(ReloadAfterDelay());
        }

        static void HaltCombat()
        {
            foreach (Enemy.Enemy enemy in Object.FindObjectsByType<Enemy.Enemy>(FindObjectsSortMode.None))
            {
                enemy.enabled = false;
                if (enemy.AudioSource != null) enemy.AudioSource.Stop();
                if (enemy.Animator != null) enemy.Animator.enabled = false;
                if (enemy.Rigidbody != null) enemy.Rigidbody.simulated = false;
            }

            foreach (WaveSpawner spawner in Object.FindObjectsByType<WaveSpawner>(FindObjectsSortMode.None))
                spawner.StopAllCoroutines();
        }

        private IEnumerator ReloadAfterDelay()
        {
            yield return new WaitForSecondsRealtime(ReloadDelay);
            Time.timeScale = 1f;

            string currentScene = SceneManager.GetActiveScene().name;
            if (SceneTransitioner.Instance != null)
                SceneTransitioner.Instance.LoadSceneWithCrossfade(currentScene, 0f, writeSave: false);
            else
                SceneManager.LoadScene(currentScene);
        }

        public override void HandleHeal(Player owner, int amount) { }

        public override void HandleDamage(Player owner, int amount) { }
    }
}
