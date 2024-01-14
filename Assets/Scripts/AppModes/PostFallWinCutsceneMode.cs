public class PostFallWinCutsceneMode : CutsceneMode
{
	public static PostFallWinCutsceneMode Instance;
    public PostFallWinCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
	    // get dialogue
	    _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelWinText;
	    base.OnEnter();
    }

    public override string Name => "Post-Fall Cutscene";
}