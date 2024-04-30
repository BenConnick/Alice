public class FallingGameActiveMode : AppMode
{
    public FallingGameActiveMode() : base()
    {
    }

    public override void OnEnter()
    {
        FallingGameInstance.Current.OnShow();
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        FallingGameInstance.Current.OnHide();
    }

    public override string Name => "Gameplay";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        FallingGameInstance.Current.HandlePlayerInput(inputType);
        return true;
    }

    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
        return FallingGameInstance.Current.HandleGlobalEvent(gameEvent);
    }
}