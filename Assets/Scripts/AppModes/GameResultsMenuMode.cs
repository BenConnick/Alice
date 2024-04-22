public class GameResultsMenuMode : AppMode
{
    public GameResultsMenuMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        GameHelper.AllGameInstances(i =>
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

    public override string Name => throw new System.NotImplementedException();

    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        return true;
    }
}