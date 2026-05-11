using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public override string ID => "Dead";
    
    /// <summary>
    /// <c>Enter</c> shuts everything down.
    /// This includes clearing the player's current velocity to 0 (preventing them from walking),
    /// stopping the audio source, and resetting the animator back to an idle pose.
    /// </summary>
    /// <param name="owner"></param>
    public override void Enter(Player owner)
    {
        owner.CurrentVelocity = Vector2.zero;
        owner.AudioSource.Stop();
        owner.Animator.SetFloat(Player.SpeedHash, 0f);
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
