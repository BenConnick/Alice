public class TitleMenuMode : AppMode
{
	public TitleMenuMode() : base()
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
        World.Get<MainUIController>().SetGameViewHidden();
    }

    public override string Name => "Title";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        switch (inputType)
        {
            case ContextualInputSystem.InputType.MouseUp:
                if (IsTitleAnimationDone())
                {
                    GameplayManager.Fire(GlobalGameEvent.MainMenuGoNext);
                }
                else
                {
                    ContextualInputSystem.ActiveGameInstance.FastForwardTitleIntro();
                }
                break;  
        }

        return true;
    }

    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
        return false;
    }

    private bool IsTitleAnimationDone()
    {
        bool done = false;
        if (ContextualInputSystem.ActiveViewport != null && ContextualInputSystem.ActiveViewport.GameplayCamera != null)
            done = ContextualInputSystem.ActiveViewport.GameplayCamera.transform.localPosition.y == 0; // kinda hacky
        return done;
    }
}