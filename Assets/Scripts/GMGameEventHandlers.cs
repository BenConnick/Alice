using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

// WARNING: Reflection is in use, function names matter!!!

// Game Manager
// partial class pertaining to responding to events within the game
// contains a lot of specific logic for particular events
// 
public static partial class GM
{
    public enum NavigationEvent
    {
        Default,
        MainMenuGoNext,
        PlatformerGameOver,
        PlatformerLevelEndTrigger,
        PlatformerLevelEndPostAnimation,
        CheatCodeEntered,
        FallFromMonologue,
        SplitAnimationMidPoint,
        GameOverGoNext,
        DialogueGoNext,
    }
    
    public static void OnGameEvent(NavigationEvent gameEvent)
    {
        switch (gameEvent)
        {
            case NavigationEvent.MainMenuGoNext:
                {
                    DoFirstStart();
                    FindSingle<Alice>().UnbecomeButton();
                }

                break;
            case NavigationEvent.PlatformerGameOver:
                {
                    DeathCount++;
                    IsGameplayPaused = true;
                    //string storyKey = PassageStartPrefix + (DeathCount < 2 ? "" : " " + DeathCount);
                    //ShowStory(storyKey);

                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(true);
                    });
                }

                break;
            case NavigationEvent.PlatformerLevelEndTrigger:
                {
                    FindSingle<RabbitHole>().PlayOutroAnimation();
                }
                break;
            case NavigationEvent.FallFromMonologue:
                {
                    AllDisplays(rhd =>
                    {
                        var rh = rhd.ObstacleContext;
                        rh.Reset();
                        rh.PlayIntroAnimationForRestart();
                    });                    
                    IsGameplayPaused = false;
                    FindSingle<GameplayScreenBehavior>().ShowGame();
                }
                break;
            case NavigationEvent.SplitAnimationMidPoint:
                {
                    PlayGameInner();
                }
                break;
            case NavigationEvent.GameOverGoNext:
                {
                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(false);
                    });
                    OnGameEvent(NavigationEvent.FallFromMonologue);
                }
                break;
            case NavigationEvent.DialogueGoNext:
                {
                    AdvanceDialogueContext();
                }
                break;
        }
    }

    private static void DoFirstStart()
    {
        CurrentMode = GameMode.Gameplay;
        CurrentLevel = LevelType.RabbitHole;
        PlayGameInner();
    }

    private static void AllDisplays(Action<RabbitHoleDisplay> dispAction)
    {
        foreach (var disp in RabbitHoleDisplay.All)
        {
            dispAction(disp);
        }
    }

    private static void BeginDialogue()
    {
        CurrentMode = GameMode.Dialogue;
        CurrentLevel++;
        IsGameplayPaused = true;
        FindSingle<Alice>().BecomeButton();
        AdvanceDialogueContext();
    }

    private static void PlayGameInner()
    {
        FindSingle<Alice>().UnbecomeButton();
        IsGameplayPaused = false;
        TimeDistortionController.SetBaselineSpeed(GetLevelTimeScale(CurrentLevel));
        foreach (var disp in RabbitHoleDisplay.All)
        {
            var hole = disp.ObstacleContext;
            hole.Reset();
            hole.PlayIntroAnimationForCurrentLevel();
        }
    }

    private static void PlayCaterpillarDoneMoment()
    {
        FindSingle<SplitGameplayMomentAnimationController>().RevealSecondView();
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
                PlayGameInner();
        }
    }

    /*private static void ShowStory(string storyKey)
    {
        FindSingle<GameplayScreenBehavior>().ShowStory(storyKey);
        CurrentMode = GameMode.Dialogue;
    }*/
}