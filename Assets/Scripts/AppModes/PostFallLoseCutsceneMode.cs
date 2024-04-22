using UnityEngine;

public class PostFallLoseCutsceneMode : CutsceneMode
{
    public PostFallLoseCutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        // get dialogue
        _TextToShow = GameHelper.GetCurrentLevelConfig().LevelText.Data.LevelLoseText;
        base.OnEnter();
        
        
        // set alice position
        var alice = Root.Find<AliceCharacter>();
        var fallingGameInstance = alice.gameContext;
        if (fallingGameInstance != null)
        {
            Vector3 center = fallingGameInstance.Viewport.GetCursorViewportWorldPos(new Vector2(.5f, .5f));
            alice.transform.position = center + new Vector3(2.5f, -2.5f, 0);
            alice.ActivateMenuMode();
        }
    }

    public override string Name => "Post-Fall Cutscene";
}