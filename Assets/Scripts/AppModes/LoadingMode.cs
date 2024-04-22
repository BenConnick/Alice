public class LoadingMode : AppMode
{
	public LoadingMode(StateMachine<AppMode> owner) : base(owner)
    {
        
    }

    public override void OnEnter()
    {
        
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override string Name => "Loading";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
	    return false;
    }
}