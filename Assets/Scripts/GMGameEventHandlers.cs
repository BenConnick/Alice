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
        FallFromMonologue,
        SplitAnimationMidPoint,
        GameOverGoNext,
        DialogueGoNext,
        BedInteraction,
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
                    CurrentMode = GameMode.GameOver;
                    DeathCount++;
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
                    // play outro
                    AllDisplays(rhd => rhd.ObstacleContext.PlayOutroAnimation());
                }
                break;
            case NavigationEvent.PlatformerLevelEndPostAnimation:
                {
                    // begin dialogue
                    CurrentMode = GameMode.Dialogue;
                    CurrentLevel++;
                    FindSingle<Alice>().BecomeButton();
                    AdvanceDialogueContext();
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
                    Debug.Log("GameOverGoNext");
                    CurrentMode = GameMode.Gameplay;
                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(false);
                        var rh = disp.ObstacleContext;
                        rh.Reset();
                        rh.PlayIntroAnimationForRestart();
                    });
                }
                break;
            case NavigationEvent.DialogueGoNext:
                {
                    AdvanceDialogueContext();
                }
                break;
            case NavigationEvent.BedInteraction:
                {
                    Debug.Log("BedInteraction");
                    CurrentMode = GameMode.Gameplay;
                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(false);
                        var rh = disp.ObstacleContext;
                        rh.Reset();
                        rh.PlayIntroAnimationForRestart();
                    });
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

    private static void PlayGameInner()
    {
        CurrentMode = GameMode.Gameplay;
        FindSingle<Alice>().UnbecomeButton();
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