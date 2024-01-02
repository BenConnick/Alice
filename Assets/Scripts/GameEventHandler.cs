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
    DialogueGoNext = 8,
    BedInteraction = 9,
    MenuAnimationFinished = 10,
    BootLoadFinished = 11,
}
    
public static class GameEventHandler
{
    #region game event handlers

    public static void OnGameEvent(NavigationEvent gameEvent)
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
                GlobalObjects.FindSingle<AliceCharacter>().UnbecomeButton();
                BeginGameplayMode();
                break;
            }
            case NavigationEvent.PlatformerGameOver:
            {
                ApplicationLifetime.ChangeMode(PostFallCutsceneMode.Instance);
                // death count?
                //string storyKey = PassageStartPrefix + (DeathCount < 2 ? "" : " " + DeathCount);
                //ShowStory(storyKey);

                AllGameInstances(i =>
                {
                    i.UIOverlay.GameOverOverlay.SetActive(true);
                });
                break;
            }
            case NavigationEvent.PlatformerLevelEndTrigger:
            {
                // play outro
                AllGameInstances(i => i.PlayOutroAnimation());
                break;
            }
            case NavigationEvent.PlatformerLevelEndPostAnimation:
            {
                // begin dialogue
                ApplicationLifetime.ChangeMode(PostFallCutsceneMode.Instance);
                LevelType val = (LevelType)ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
                ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(val);
                GlobalObjects.FindSingle<AliceCharacter>().BecomeButton();
                AdvanceDialogueContext();
                break;
            }
            case NavigationEvent.FallFromMonologue:
            {
                AllGameInstances(i =>
                {
                    i.Reset();
                    i.PlayIntroAnimationForRestart();
                });
                GlobalObjects.FindSingle<GameplayScreenBehavior>().ShowGame();
                break;
            }
            case NavigationEvent.SplitAnimationMidPoint:
            {
                BeginGameplayMode();
            }
                break;
            case NavigationEvent.GameOverGoNext:
            {
                Debug.Log("GameOverGoNext");

                // reset all displays
                AllGameInstances(i =>
                {
                    i.UIOverlay.GameOverOverlay.SetActive(false);
                    i.Reset();
                    i.menuGraphics.ShowStageArt(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
                    i.PlayTitleIntro();
                });

                // set mode to main menu
                ApplicationLifetime.ChangeMode(TitleMenuMode.Instance);

                // set alice position
                var alice = GlobalObjects.FindSingle<AliceCharacter>();
                var fallingGameInstance = alice.gameContext;
                if (fallingGameInstance != null)
                {
                    Vector3 center = fallingGameInstance.Viewport.GetCursorViewportWorldPos(new Vector2(.5f, .5f));
                    alice.transform.position = center + new Vector3(2.5f, -2.5f, 0);
                    alice.BecomeButton();
                }
                
                break;
            }
            case NavigationEvent.DialogueGoNext:
            {
                AdvanceDialogueContext();
                break;
            }
            case NavigationEvent.BedInteraction:
            {
                Debug.Log("BedInteraction");
                ApplicationLifetime.ChangeMode(FallingGameActiveMode.Instance);
                AllGameInstances(gameInstance =>
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
        }
    }

    private static void AllGameInstances(Action<FallingGameInstance> eachInstanceAction)
    {
        foreach (FallingGameInstance instance in FallingGameInstance.All)
        {
            eachInstanceAction(instance);
        }
    }

    private static void BeginGameplayMode()
    {
        ApplicationLifetime.ChangeMode(FallingGameActiveMode.Instance);
        GlobalObjects.FindSingle<AliceCharacter>().UnbecomeButton();
        TimeDistortionController.SetBaselineSpeed(FallingGameInstance.Current.Config.TimeScaleMultiplier);
        AllGameInstances(i =>
        {
            i.Reset();
            i.PlayIntroAnimationForCurrentLevel();
        });
    }

    public static void PlayCaterpillarDoneMoment()
    {
        GlobalObjects.FindSingle<SplitGameplayMomentAnimationController>().RevealSecondView();
    }

    private static void AdvanceDialogueContext()
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
                BeginGameplayMode();
        }
    }
    #endregion
}