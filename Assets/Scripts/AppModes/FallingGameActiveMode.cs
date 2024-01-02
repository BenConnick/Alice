public class FallingGameActiveMode : AppMode
{
	public static FallingGameActiveMode Instance;
    public FallingGameActiveMode(StateMachine<AppMode> owner) : base(owner)
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

    public override string Name => "Gameplay";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }
}