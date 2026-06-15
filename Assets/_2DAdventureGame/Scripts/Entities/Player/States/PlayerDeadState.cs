using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadState : PlayerState
{
    private const float ReloadDelay = 3f;

    public override void Enter(Player owner)
    {
        owner.CurrentVelocity = Vector2.zero;
        owner.Animator.SetFloat(AnimatorHashes.Speed, 0f);

        Time.timeScale = 0f;

        if (UIHandler.Instance != null) UIHandler.Instance.DisplayLoseScreen();
        owner.StartCoroutine(ReloadAfterDelay());
    }

    private IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(ReloadDelay);
        Time.timeScale = 1f;

        if (Player.Instance != null)
            Player.Instance.CurrentHealth = Player.Instance.Data.maxHealth;

        string currentScene = SceneManager.GetActiveScene().name;
        if (SceneTransitioner.Instance != null)
            SceneTransitioner.Instance.LoadSceneWithCrossfade(currentScene, 0f);
        else
            SceneManager.LoadScene(currentScene);
    }

    public override void HandleHeal(Player owner, int amount) { }

    public override void HandleDamage(Player owner, int amount) { }
}
