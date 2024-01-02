public abstract class CutsceneMode : AppMode
{
    public CutsceneMode(StateMachine<AppMode> owner) : base(owner)
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

    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }
}