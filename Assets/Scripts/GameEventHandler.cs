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
        switch (gameEvent)
        {
            case NavigationEvent.MainMenuGoNext:
            {
                GM.FindSingle<Alice>().UnbecomeButton();
                BeginGameplayMode();
                break;
            }
            case NavigationEvent.PlatformerGameOver:
            {
                GM.CurrentMode = GM.GameMode.GameOver;
                GM.DeathCount++;
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
                GM.CurrentMode = GM.GameMode.Dialogue;
                GM.CurrentLevel++;
                GM.FindSingle<Alice>().BecomeButton();
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
                GM.FindSingle<GameplayScreenBehavior>().ShowGame();
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
                GM.CurrentMode = GM.GameMode.Gameplay;

                // reset all displays
                AllDisplays(disp =>
                {
                    GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                    gameOverUI.SetActive(false);
                    var rh = disp.ObstacleContext;
                    rh.Reset();
                    rh.menuGraphics.ShowStageArt(GM.CurrentLevel);
                    rh.PlayTitleIntro();
                });

                // set mode to main menu
                GM.CurrentMode = GM.GameMode.PreMainMenu;

                // set alice position
                var alice = GM.FindSingle<Alice>();
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
                GM.CurrentMode = GM.GameMode.Gameplay;
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
                GM.CurrentMode = GM.GameMode.MainMenu;
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
        GM.CurrentMode = GM.GameMode.Gameplay;
        GM.FindSingle<Alice>().UnbecomeButton();
        TimeDistortionController.SetBaselineSpeed(GetLevelTimeScale(GM.CurrentLevel));
        foreach (var disp in RabbitHoleDisplay.All)
        {
            var hole = disp.ObstacleContext;
            hole.Reset();
            hole.PlayIntroAnimationForCurrentLevel();
        }
    }

    private static float GetLevelTimeScale(LevelType level)
    {
        return level == LevelType.Caterpillar ? .5f : 1f;
    }

    public static void PlayCaterpillarDoneMoment()
    {
        GM.FindSingle<SplitGameplayMomentAnimationController>().RevealSecondView();
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

    /*private static void ShowStory(string storyKey)
        {
            FindSingle<GameplayScreenBehavior>().ShowStory(storyKey);
            GM.CurrentMode = GameMode.Dialogue;
        }*/
    #endregion
}