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
    GameResultsClosed = 7,
    LegacyDialogueGoNext = 8,
    BedInteraction = 9,
    MenuAnimationFinished = 10,
    BootLoadFinished = 11,
    PreRunCutsceneFinished = 12,
    PostRunCutsceneFinished = 13,
    LevelSelectionConfirmed = 14,
    Temp = 15,
}