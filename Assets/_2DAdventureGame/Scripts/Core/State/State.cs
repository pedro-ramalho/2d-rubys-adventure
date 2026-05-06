using System;

/// <summary>
/// Abstract class for modeling a generic state.
/// </summary>
/// <typeparam name="T">The type of the owner of the state.</typeparam>
public abstract class State<T, TID> where TID : Enum
{
    /// <summary>
    /// 
    /// </summary>
    public abstract TID ID { get; }

    /// <summary>
    /// Method that is executed when a transition happens <b>to</b> this state.
    /// It should be used for setting up the necessary context for the state to operate as intended.
    /// </summary>
    /// <param name="owner">A reference to the owner of the state's context.</param>
    public abstract void Enter(T owner);

    /// <summary>
    /// Method that is executed on every frame whilst the state is active.
    /// </summary>
    /// <param name="owner">A reference to the owner of the state's context.</param>
    public abstract void Update(T owner);
    
    /// <summary>
    /// Called every fixed timestep while this state is active. Use it for
    /// physics-driven behaviour such as moving a Rigidbody.
    /// </summary>
    public abstract void FixedUpdate(T owner);

    /// <summary>
    /// Method that is executed when a transition happens <b>from</b> this state.
    /// It should be used for clearing up the context for the state machine to remain operating normally.
    /// </summary>
    /// <param name="owner">A reference to the owner of the state's context.</param>
    public abstract void Exit(T owner); 
}
