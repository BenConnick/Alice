public class PostFallWinCutsceneMode : CutsceneMode
{
	public PostFallWinCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
	    // get dialogue
	    _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelWinText;
	    base.OnEnter();

	    Root.Find<AliceCharacter>().ActivateMenuMode();
	    GameHelper.UnlockNextLevel();
    }

    public override string Name => "Post-Fall Cutscene";
}