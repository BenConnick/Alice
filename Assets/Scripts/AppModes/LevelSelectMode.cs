public class LevelSelectMode : AppMode
{
	public LevelSelectMode() : base()
    {
    }

    public override void OnEnter()
    {
        World.Get<GameplayScreenBehavior>().ShowLevelSelect();
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        World.Get<GameplayScreenBehavior>().HideLevelSelect();
    }

    public override string Name => "Level Select";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }

    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
        return false;
    }
}