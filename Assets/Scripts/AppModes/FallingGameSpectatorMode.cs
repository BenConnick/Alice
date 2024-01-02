public class FallingGameSpectatorMode : AppMode
{
	public static FallingGameSpectatorMode Instance;
    public FallingGameSpectatorMode(StateMachine<AppMode> owner) : base(owner)
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

    public override string Name => "Spectating";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return false;
    }
}