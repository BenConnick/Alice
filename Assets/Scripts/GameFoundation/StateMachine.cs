using System;
using System.Collections.Generic;

public class StateMachine<T> where T : StateMachine<T>.State
{
    public abstract class State
    {
        protected StateMachine<T> Owner;
        
        protected State(StateMachine<T> owner)
        {
            Owner = owner;
            Owner.RegisteredStates.Add(typeof(T), (T)this);
        }

        ~State()
        {
            Owner.RegisteredStates.Remove(GetType());
        }
        
        public abstract void OnEnter();

        public abstract void Tick(float dt);

        public abstract void OnExit();
    }

    public virtual Dictionary<Type, T> RegisteredStates { get; protected set; }  = new Dictionary<Type, T>();

    public virtual TState Get<TState>() where TState : T
    {
        if (RegisteredStates.TryGetValue(typeof(TState), out T state))
        {
            return (TState)state;
        }
        return default;
    }

    public virtual T CurrentState { get; protected set; }

    public void ChangeState(T newState)
    {
        if (CurrentState != null)
        {
            CurrentState.OnExit();
        }

        CurrentState = newState;
        newState.OnEnter();
    }

    public void Tick(float dt)
    {
        CurrentState.Tick(dt);
    }
}