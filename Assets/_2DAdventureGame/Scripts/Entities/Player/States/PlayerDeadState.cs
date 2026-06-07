using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadState : PlayerState
{
    private const float ReloadDelay = 5f;

    /// <summary>
    /// <c>Enter</c> shuts everything down.
    /// This includes clearing the player's current velocity to 0 (preventing them from walking),
    /// stopping the audio source, and resetting the animator back to an idle pose.
    /// </summary>
    /// <param name="owner"></param>
    public override void Enter(Player owner)
    {
        owner.CurrentVelocity = Vector2.zero;
        owner.Animator.SetFloat(Player.SpeedHash, 0f);

        Time.timeScale = 0f;

        if (UIHandler.Instance != null) UIHandler.Instance.DisplayLoseScreen();
        owner.StartCoroutine(ReloadAfterDelay());
    }

    private IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(ReloadDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Empty, since there are no input readings or state transitions during this state.
    /// </summary>
    /// <param name="owner">Unused.</param>
    public override void Update(Player owner) { }

    /// <summary>
    /// Empty, since there are no input readings or state transitions during this state.
    /// </summary>
    /// <param name="owner">Unused</param>
    public override void FixedUpdate(Player owner) { }

    /// <summary>
    /// Empty, since there are no state transitions from this state.
    /// </summary>
    /// <param name="owner">Unused.</param>
    public override void Exit(Player owner) { }

    /// <summary>
    /// Empty, since we want to prevent the Player from healing whilst dead.
    /// </summary>
    /// <param name="owner">Unused.</param>
    /// <param name="amount">Unused.</param>
    public override void HandleHeal(Player owner, int amount) { }

    /// <summary>
    /// Empty, since we want to prevent the Player from taking damage whilst dead.
    /// </summary>
    /// <param name="owner">Unused.</param>
    /// <param name="amount">Unused.</param>
    public override void HandleDamage(Player owner, int amount) { }
}
