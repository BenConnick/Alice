using System;
using UnityEngine;

public enum GlobalGameEvent
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
    
public static class GameEvents
{
    public static void Report(GlobalGameEvent gameEvent)
    {
        ApplicationLifetime.HandleGlobalGameEvent(gameEvent);
    }

    public static void SelectLevel(LevelType level)
    {
        ApplicationLifetime.ChangeSelectedLevel(level);
        ApplicationLifetime.ChangeMode<FallingGameActiveMode>();
    }
}