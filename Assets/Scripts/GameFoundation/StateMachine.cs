using System;
using System.Collections.Generic;

public class StateMachine<T> where T : StateMachine<T>.State
{
    public abstract class State
    {
        public abstract void OnEnter();

        public abstract void Tick(float dt);

        public abstract void OnExit();
    }

    protected virtual Dictionary<Type, T> RegisteredStates { get; set; }  = new Dictionary<Type, T>();

    public IEnumerable<State> ForAllStates()
    {
        foreach (var kv in RegisteredStates)
        {
            yield return kv.Value;
        }
    }

    public virtual TState Get<TState>() where TState : T, new()
    {
        if (!RegisteredStates.ContainsKey(typeof(TState)))
        {
            RegisteredStates.Add(typeof(TState), new TState());
        }
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