using System;
using UnityEngine;

public enum NavigationEvent
{
    Default = 0,
    MainMenuGoNext = 1,
    AllLivesLost = 2,
    PlatformerLevelEndReached = 3,
    PlatformerLevelEndAnimationFinished = 4,
    FallFromMonologue = 5,
    SplitAnimationMidPoint = 6,
    OnGameResultsClosed = 7,
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
                ApplicationLifetime.ChangeMode<TitleMenuMode>();
                break;
            }
            case NavigationEvent.MainMenuGoNext:
            {
                ApplicationLifetime.ChangeMode<PreFallCutsceneMode>();
                break;
            }
            case NavigationEvent.AllLivesLost:
            {
                ApplicationLifetime.ChangeMode<FallingGameSpectatorMode>();
                break;
            }
            case NavigationEvent.PlatformerLevelEndReached:
            {
                // play outro
                GameHelper.AllGameInstances(i => i.PlayOutroAnimation());
                break;
            }
            case NavigationEvent.PlatformerLevelEndAnimationFinished:
            {
                // begin dialogue
                ApplicationLifetime.ChangeMode<PostFallWinCutsceneMode>();;
                break;
            }
            case NavigationEvent.OnGameResultsClosed:
            {
                // set mode to main menu
                ApplicationLifetime.ChangeMode<PostFallLoseCutsceneMode>();
                break;
            }
            case NavigationEvent.MenuAnimationFinished:
            {
                Debug.Log("MenuAnimationFinished");
                break;
            }
            case NavigationEvent.PreRunDialogueFinished:
            {
                ApplicationLifetime.ChangeMode<FallingGameActiveMode>();
                break;
            }
            case NavigationEvent.PostRunDialogueFinished:
            {
                ApplicationLifetime.ChangeMode<LevelSelectMode>();
                break;
            }
        }
    }

    public static void GoToLevel(LevelType level)
    {
        ApplicationLifetime.ChangeSelectedLevel(level);
        ApplicationLifetime.ChangeMode<FallingGameActiveMode>();
    }

    #endregion
}