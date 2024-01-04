public class StateMachine<T> where T : StateMachine<T>.State
{
    public abstract class State
    {
        protected StateMachine<T> owner;
        
        protected State(StateMachine<T> owner)
        {
            this.owner = owner;
        }
        
        public abstract void OnEnter();

        public abstract void Tick(float dt);

        public abstract void OnExit();
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