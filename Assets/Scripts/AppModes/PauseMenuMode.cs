public class PauseMenuMode : AppMode
{
	public PauseMenuMode(StateMachine<AppMode> owner) : base(owner)
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

    public override string Name => "Paused";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        throw new System.NotImplementedException();
    }
}