using System;
using UnityEngine;

public enum NavigationEvent
{
    Default = 0,
    MainMenuGoNext = 1,
    PlatformerGameOver = 2,
    PlatformerLevelEndTrigger = 3,
    PlatformerLevelEndPostAnimation = 4,
    FallFromMonologue = 5,
    SplitAnimationMidPoint = 6,
    GameOverGoNext = 7,
    LegacyDialogueGoNext = 8,
    BedInteraction = 9,
    MenuAnimationFinished = 10,
    BootLoadFinished = 11,
    PreRunDialogueFinished = 12,
    PostRunDialogueFinished = 13,
}
    
public static class Nav
{
    #region game event handlers

    public static void Go(NavigationEvent gameEvent)
    {
        Debug.Log("On game event: " + gameEvent);
        switch (gameEvent)
        {
            case NavigationEvent.BootLoadFinished:
            {
                ApplicationLifetime.ChangeMode(TitleMenuMode.Instance);
                break;
            }
            case NavigationEvent.MainMenuGoNext:
            {
                Root.Find<AliceCharacter>().UnbecomeButton();
                ApplicationLifetime.ChangeMode(PreFallCutsceneMode.Instance);
                break;
            }
            case NavigationEvent.PlatformerGameOver:
            {
                GameHelper.AllGameInstances(i =>
                {
                    i.UIOverlay.GameOverOverlay.SetActive(true);
                });
                break;
            }
            case NavigationEvent.PlatformerLevelEndTrigger:
            {
                // play outro
                GameHelper.AllGameInstances(i => i.PlayOutroAnimation());
                break;
            }
            case NavigationEvent.PlatformerLevelEndPostAnimation:
            {
                // begin dialogue
                ApplicationLifetime.ChangeMode(PostFallWinCutsceneMode.Instance);
                LevelType val = (LevelType)ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
                ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(val);
                Root.Find<AliceCharacter>().BecomeButton();
                LegacyAdvanceDialogueContext();
                break;
            }
            case NavigationEvent.FallFromMonologue:
            {
                GameHelper.AllGameInstances(i =>
                {
                    i.Reset();
                    i.PlayIntroAnimationForRestart();
                });
                Root.Find<GameplayScreenBehavior>().ShowGame();
                break;
            }
            case NavigationEvent.SplitAnimationMidPoint:
            {
                LegacyBeginGameplayMode();
            }
                break;
            case NavigationEvent.GameOverGoNext:
            {
                Debug.Log("GameOverGoNext");

                // reset all displays
                GameHelper.AllGameInstances(i =>
                {
                    i.UIOverlay.GameOverOverlay.SetActive(false);
                    i.Reset();
                    i.menuGraphics.ShowStageArt(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
                    i.PlayTitleIntro();
                });

                // set mode to main menu
                ApplicationLifetime.ChangeMode(PostFallLoseCutsceneMode.Instance);

                // set alice position
                var alice = Root.Find<AliceCharacter>();
                var fallingGameInstance = alice.gameContext;
                if (fallingGameInstance != null)
                {
                    Vector3 center = fallingGameInstance.Viewport.GetCursorViewportWorldPos(new Vector2(.5f, .5f));
                    alice.transform.position = center + new Vector3(2.5f, -2.5f, 0);
                    alice.BecomeButton();
                }
                
                break;
            }
            case NavigationEvent.LegacyDialogueGoNext:
            {
                LegacyAdvanceDialogueContext();
                break;
            }
            case NavigationEvent.BedInteraction:
            {
                Debug.Log("BedInteraction");
                ApplicationLifetime.ChangeMode(FallingGameActiveMode.Instance);
                GameHelper.AllGameInstances(gameInstance =>
                {
                    GameObject gameOverUI = gameInstance.UIOverlay.GameOverOverlay;
                    gameOverUI.SetActive(false);
                    gameInstance.Reset();
                    gameInstance.PlayIntroAnimationForRestart();
                });
                break;
            }
            case NavigationEvent.MenuAnimationFinished:
            {
                Debug.Log("MenuAnimationFinished");
                break;
            }
            case NavigationEvent.PreRunDialogueFinished:
            {
                BeginGameplay();
                break;
            }
            case NavigationEvent.PostRunDialogueFinished:
            {
                ApplicationLifetime.ChangeMode(LevelSelectMode.Instance);
                break;
            }
        }
    }

    private static void LegacyBeginGameplayMode()
    {
        Root.Find<AliceCharacter>().UnbecomeButton();
        BeginGameplay();
    }

    private static void BeginGameplay()
    {
        ApplicationLifetime.ChangeMode(FallingGameActiveMode.Instance);
    }

    public static void PlayCaterpillarDoneMoment()
    {
        Root.Find<SplitGameplayMomentAnimationController>().RevealSecondView();
    }

    private static void LegacyAdvanceDialogueContext()
    {
        // get current dialogue context
        var ctx = CharacterDialogueBehavior.ActiveDialogue;

        if (ctx == null)
        {
            Debug.LogError("Dialogue advanced, but no dialogue active");
            return;
        }

        // advance to next step
        bool last = ctx.PlayNextLine();
        // if this is the last step, start the next falling section
        if (last)
        {
            if (ctx is CaterpillarDialogueBehavior)
                PlayCaterpillarDoneMoment();
            else
                LegacyBeginGameplayMode();
        }
    }
    #endregion
}