public class FallingGameActiveMode : AppMode
{
    private FallingGameInstance gameInstance;

    public FallingGameActiveMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        Root.Find<GameplayScreenBehavior>().ShowGame();
        Root.Find<AliceCharacter>().ActivateGameplayMode();
        TimeDistortionController.SetBaselineSpeed(FallingGameInstance.Current.Config.TimeScaleMultiplier);
        GameHelper.AllGameInstances(i =>
        {
            i.Reset();
            i.PlayIntroAnimationForCurrentLevel();
            gameInstance = i;
        });
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        Root.Find<GameplayScreenBehavior>().HideGame();
        gameInstance.Reset();
        gameInstance.UIOverlay.GameOverOverlay.SetActive(false);
        LevelType levelValue = ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value;
        gameInstance.menuGraphics.ShowStageArt(levelValue);
        gameInstance.PlayTitleIntro();
    }

    public override string Name => "Gameplay";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }
}