using System;
using UnityEngine;

public enum NavigationEvent
{
    Default,
    MainMenuGoNext,
    PlatformerGameOver,
    PlatformerLevelEndTrigger,
    PlatformerLevelEndPostAnimation,
    FallFromMonologue,
    SplitAnimationMidPoint,
    GameOverGoNext,
    DialogueGoNext,
    BedInteraction,
    MenuAnimationFinished
}
    
public static class GameEventHandler
{
    #region game event handlers

    public static void OnGameEvent(NavigationEvent gameEvent)
    {
        Debug.Log("On game event: " + gameEvent);
        switch (gameEvent)
        {
            case NavigationEvent.MainMenuGoNext:
            {
                GlobalObjects.FindSingle<AliceCharacter>().UnbecomeButton();
                BeginGameplayMode();
                break;
            }
            case NavigationEvent.PlatformerGameOver:
            {
                ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.GameOver;
                // death count?
                //string storyKey = PassageStartPrefix + (DeathCount < 2 ? "" : " " + DeathCount);
                //ShowStory(storyKey);

                AllDisplays(disp =>
                {
                    GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                    gameOverUI.SetActive(true);
                });
                break;
            }
            case NavigationEvent.PlatformerLevelEndTrigger:
            {
                // play outro
                AllDisplays(rhd => rhd.ObstacleContext.PlayOutroAnimation());
                break;
            }
            case NavigationEvent.PlatformerLevelEndPostAnimation:
            {
                // begin dialogue
                ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.Dialogue;
                LevelType val = (LevelType)ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
                ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(val);
                GlobalObjects.FindSingle<AliceCharacter>().BecomeButton();
                AdvanceDialogueContext();
                break;
            }
            case NavigationEvent.FallFromMonologue:
            {
                AllDisplays(rhd =>
                {
                    var rh = rhd.ObstacleContext;
                    rh.Reset();
                    rh.PlayIntroAnimationForRestart();
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
                ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.Gameplay;

                // reset all displays
                AllDisplays(disp =>
                {
                    GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                    gameOverUI.SetActive(false);
                    var rh = disp.ObstacleContext;
                    rh.Reset();
                    rh.menuGraphics.ShowStageArt(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
                    rh.PlayTitleIntro();
                });

                // set mode to main menu
                ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.PreMainMenu;

                // set alice position
                var alice = GlobalObjects.FindSingle<AliceCharacter>();
                var ctx = alice.movementContext;
                if (ctx != null)
                {
                    Vector3 center = ctx.GetCursorViewportWorldPos(new Vector2(.5f, .5f));
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
                ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.Gameplay;
                AllDisplays(disp =>
                {
                    GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                    gameOverUI.SetActive(false);
                    var rh = disp.ObstacleContext;
                    rh.Reset();
                    rh.PlayIntroAnimationForRestart();
                });
                break;
            }
            case NavigationEvent.MenuAnimationFinished:
            {
                Debug.Log("MenuAnimationFinished");
                ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.MainMenu;
                break;
            }
        }
    }

    private static void AllDisplays(Action<RabbitHoleDisplay> dispAction)
    {
        foreach (var disp in RabbitHoleDisplay.All)
        {
            dispAction(disp);
        }
    }

    private static void BeginGameplayMode()
    {
        ApplicationLifetime.CurrentMode = ApplicationLifetime.GameMode.Gameplay;
        GlobalObjects.FindSingle<AliceCharacter>().UnbecomeButton();
        TimeDistortionController.SetBaselineSpeed(RabbitHole.Current.Config.TimeScaleMultiplier);
        foreach (var disp in RabbitHoleDisplay.All)
        {
            var hole = disp.ObstacleContext;
            hole.Reset();
            hole.PlayIntroAnimationForCurrentLevel();
        }
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