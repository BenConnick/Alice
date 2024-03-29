public class LevelSelectMode : AppMode
{
	public static LevelSelectMode Instance;
    public LevelSelectMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Root.Find<GameplayScreenBehavior>().ShowLevelSelect();
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override string Name => "Level Select";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }
}