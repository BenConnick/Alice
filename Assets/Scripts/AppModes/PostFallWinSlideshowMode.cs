public class PostFallWinSlideshowMode : SlideshowMode
{
	public PostFallWinSlideshowMode() : base()
    {
    }

    public override void OnEnter()
    {
	    base.OnEnter();

	    DialogueExhausted += () => GameplayManager.Fire(GlobalGameEvent.PostRunCutsceneFinished);
	    // additional
	    World.Get<AliceCharacter>().ActivateMenuMode();
	    GameplayManager.UnlockNextLevel();
    }

    public override string Name => "Post-Fall Slideshow";

    protected override InGameText GetSlideshowText()
    {
	    return GameplayManager.GetCurrentLevelConfig().LevelText.Data.LevelWinText;
    }
}