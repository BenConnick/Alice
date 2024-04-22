using UnityEngine;
using System;

public static partial class ApplicationLifetime
{
    public enum DebugEvent
    {
        Default,
        ShowNameEntryScreen,
        SetLevelCaterpillar,
    }

    public static void DebugOnPostInitialize()
    {
        // auto inject debug values into game state on startup
        
        // lives
        MAX_LIVES = UnityEditor.EditorPrefs.GetBool("OneLife") ? 1 : MAX_LIVES;
    }

    public static void OnDebugEvent(DebugEvent debugEvent)
    {
        Debug.Log("Debug Event: " + debugEvent);
        switch (debugEvent)
        {
            case DebugEvent.ShowNameEntryScreen:
                Debug.Log("Deprecated");
                //CurrentLevel = LevelType.RabbitHole;
                //CurrentScore = 110;
                //ChangeMode(GameMode.EnterName);
                break;
            case DebugEvent.SetLevelCaterpillar:
                GetPlayerData().LastUnlockedLevel.Set(LevelType.GardenOfChange);
                break;
            default:
                break;
        }
    }

    #region Commands

    [Command]
    public static void InfiniteHearts()
    {
        ContextualInputSystem.ActiveGameInstance.VpLives = 999;
    }


    #endregion
}