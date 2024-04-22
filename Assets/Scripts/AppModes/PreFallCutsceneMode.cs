public class PreFallCutsceneMode : CutsceneMode
{
	public PreFallCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }
    
    public override void OnEnter()
    {
        // get dialogue
        _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelStartText;
        base.OnEnter();
    }

    public override string Name => "Pre-Fall Cutscene";
}