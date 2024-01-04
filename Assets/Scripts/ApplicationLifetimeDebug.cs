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
    public static void SetLevelCaterpillar()
    {
        OnDebugEvent(DebugEvent.SetLevelCaterpillar);
    }

    [Command]
    public static void InfiniteHearts()
    {
        ContextualInputSystem.ActiveGameInstance.VpLives = 999;
    }

    [Command]
    public static void SkipCaterpillarDialogue()
    {
        Nav.PlayCaterpillarDoneMoment();
    }

    [Command]
    public static void JumpToPOI()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.EnterPlaymode();
            return;
        }
#endif
        // replace the contents of this function with the latest point of interest
        JumpToCaterpillar();

    }

    private static void JumpToCaterpillar()
    {
        GetPlayerData().LastUnlockedLevel.Set(LevelType.GardenOfChange);
        ChangeMode(FallingGameActiveMode.Instance);
        //foreach (var rabbithole in RabbitHoleDisplay.All)
        //{
        //    rabbithole.GameplayGroup.ObstacleContext.SetBackgroundSpritesForLevel((int)CurrentLevel);
        //    rabbithole.GameplayGroup.ObstacleContext.menuGraphics.ShowStageArt(CurrentLevel);
        //    //rabbithole.GameplayGroup.UIOverlay.gameObject.SetActive(false);
        //}
        //PlayGameInner();
        Nav.PlayCaterpillarDoneMoment();
    }
    
    
    #endregion
}