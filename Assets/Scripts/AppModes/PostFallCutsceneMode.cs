public class PostFallCutsceneMode : CutsceneMode
{
	public static PostFallCutsceneMode Instance;
    public PostFallCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override string Name => "Post-Fall Cutscene";
}