public class PreFallSlideshowMode : SlideshowMode
{
	public PreFallSlideshowMode() : base()
    {
    }

	public override void OnEnter()
	{
		base.OnEnter();
		DialogueExhausted += () => GameplayManager.Fire(GlobalGameEvent.PreRunCutsceneFinished);
	}

    public override string Name => "Pre-Fall Slideshow";

    protected override InGameText GetSlideshowText()
    {
	    return GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelStartText;
    }
}