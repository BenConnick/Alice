public class PostFallWinCutsceneMode : CutsceneMode
{
	public PostFallWinCutsceneMode() : base()
    {
    }

    public override void OnEnter()
    {
	    // get dialogue
	    _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelWinText;
	    base.OnEnter();

	    World.Get<AliceCharacter>().ActivateMenuMode();
	    GameHelper.UnlockNextLevel();
    }

    public override string Name => "Post-Fall Cutscene";
    public override bool HandleGameEvent(GlobalGameEvent gameEvent)
    {
	    return false;
    }
}