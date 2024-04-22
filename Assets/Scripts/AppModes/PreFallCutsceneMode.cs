public class PreFallCutsceneMode : CutsceneMode
{
	public PreFallCutsceneMode() : base()
    {
    }
    
    public override void OnEnter()
    {
        // get dialogue
        _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelStartText;
        base.OnEnter();
    }

    public override string Name => "Pre-Fall Cutscene";
    
    
    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
        return false;
    }
}