public class PreFallCutsceneMode : CutsceneMode
{
	public static PreFallCutsceneMode Instance;
    public PreFallCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override string Name => "Pre-Fall Cutscene";
}