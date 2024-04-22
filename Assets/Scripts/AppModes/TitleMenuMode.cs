public class TitleMenuMode : AppMode
{
	public TitleMenuMode(StateMachine<AppMode> owner) : base(owner)
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
        Root.Find<GameplayScreenBehavior>().HideGame();
    }

    public override string Name => "Title";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        switch (inputType)
        {
            case ContextualInputSystem.InputType.MouseUp:
                if (IsTitleAnimationDone())
                {
                    GameEvents.Report(GlobalGameEvent.MainMenuGoNext);
                }
                else
                {
                    ContextualInputSystem.ActiveGameInstance.FastForwardTitleIntro();
                }
                break;  
        }

        return true;
    }

    private bool IsTitleAnimationDone()
    {
        bool done = false;
        if (ContextualInputSystem.ActiveViewport != null && ContextualInputSystem.ActiveViewport.GameplayCamera != null)
            done = ContextualInputSystem.ActiveViewport.GameplayCamera.transform.localPosition.y == 0; // kinda hacky
        return done;
    }
}