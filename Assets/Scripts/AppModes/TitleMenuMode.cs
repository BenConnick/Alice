public class TitleMenuMode : AppMode
{
	public TitleMenuMode() : base()
    {
    }

    public override void OnEnter()
    {
        World.Get<MenuGraphics>().ShowStageArt(GameplayManager.SelectedLevel);
        World.Get<MainUIController>().SetGameViewVisible();
        World.Get<AliceCharacter>().ActivateMenuMode();
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
                    FallingGameInstance.Current.FastForwardTitleIntro();
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
        if (FallingGameInstance.Current != null && FallingGameInstance.Current.GameplayCam != null)
            done = FallingGameInstance.Current.GameplayCam.transform.localPosition.y == 0; // kinda hacky
        return done;
    }
}