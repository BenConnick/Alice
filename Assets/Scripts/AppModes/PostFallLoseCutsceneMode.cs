public class PostFallLoseCutsceneMode : CutsceneMode
{
    public static PostFallLoseCutsceneMode Instance;
    public PostFallLoseCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        // get dialogue
        _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelLoseText;
        base.OnEnter();
    }

    public override string Name => "Post-Fall Cutscene";
}