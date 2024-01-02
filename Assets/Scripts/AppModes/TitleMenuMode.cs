public class TitleMenuMode : AppMode
{
	public static TitleMenuMode Instance;
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
        
    }

    public override string Name => "Title";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        switch (inputType)
        {
            case ContextualInputSystem.InputType.MouseUp:
                bool done = ContextualInputSystem.ActiveViewport.GameplayCamera.transform.localPosition.y == 0; // kinda hacky
                if (done)
                {
                    GameEventHandler.OnGameEvent(NavigationEvent.MainMenuGoNext);
                }
                else
                {
                    ContextualInputSystem.ActiveGameInstance.FastForwardTitleIntro();
                }
                break;  
        }

        return true;
    }
}