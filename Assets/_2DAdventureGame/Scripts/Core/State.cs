using System;


public abstract class State<T> 
{
    public virtual void Enter(T owner) {}

    public virtual void Update(T owner) {}
    
    public virtual void FixedUpdate(T owner) {}

    public virtual void Exit(T owner) {} 
}
