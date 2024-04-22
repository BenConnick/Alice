public class FallingGameActiveMode : AppMode
{
    private FallingGameInstance gameInstance;

    public FallingGameActiveMode() : base()
    {
    }

    public override void OnEnter()
    { 
        gameInstance = FallingGameInstance.All[0]; // expecting one instance, list of instances is legacy structure
        gameInstance.OnShow();
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        gameInstance.OnHide();
    }

    public override string Name => "Gameplay";
    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }

    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
        return gameInstance.HandleGlobalEvent(gameEvent);
    }
}