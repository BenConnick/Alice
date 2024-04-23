using UnityEngine;

public class PostFallLoseSlideshowMode : SlideshowMode
{
    public PostFallLoseSlideshowMode() : base()
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        DialogueExhausted += () => GameplayManager.Fire(GlobalGameEvent.PostRunCutsceneFinished);

        // set alice position
        var alice = World.Get<AliceCharacter>();
        var fallingGameInstance = alice.gameContext;
        if (fallingGameInstance != null)
        {
            Vector3 viewportCenter = fallingGameInstance.Viewport.GetCursorViewportWorldPos(new Vector2(.5f, .5f));
            alice.transform.position = viewportCenter + new Vector3(2.5f, -2.5f, 0);
            alice.ActivateMenuMode();
        }
    }

    public override string Name => "Post-Fall Lose Slideshow";

    protected override InGameText GetSlideshowText()
    {
        return GameplayManager.GetCurrentLevelConfig().LevelText.Data.LevelWinText;
    }
}