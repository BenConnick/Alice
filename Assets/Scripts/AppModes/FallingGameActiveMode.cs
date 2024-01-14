public class FallingGameActiveMode : AppMode
{
	public static FallingGameActiveMode Instance;
    public FallingGameActiveMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Root.Find<GameplayScreenBehavior>().ShowGame();
        TimeDistortionController.SetBaselineSpeed(FallingGameInstance.Current.Config.TimeScaleMultiplier);
        GameHelper.AllGameInstances(i =>
        {
            i.Reset();
            i.PlayIntroAnimationForCurrentLevel();
        });
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        Root.Find<GameplayScreenBehavior>().HideGame();
    }

    public override string Name => "Gameplay";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }
}