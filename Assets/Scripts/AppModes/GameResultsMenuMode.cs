public class GameResultsMenuMode : AppMode
{
    public GameResultsMenuMode() : base()
    {
    }

    public override void OnEnter()
    {
        GameplayManager.AllGameInstances(i =>
        {
            i.UIOverlay.GameOverOverlay.SetActive(true);
        });
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override string Name => "Game Over Results Screen";

    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }

    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
        return false;
    }
}