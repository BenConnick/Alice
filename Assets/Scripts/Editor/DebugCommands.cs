using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DebugCommands
{
    [Command]
    public static void ZQX()
    {
        // replace me when you have a platform
    }

    [Command]
    public static void EditImage()
    {
        PixelEditorWindow.ShowPixelEditor();
    }

    [Command]
    public static void CreateNewImage()
    {
        NewImageFileWindow.ShowCreateImageWindow();
    }

    [Command]
    public static void PlayFastDebug()
    {
        EditorPrefs.SetBool("ShortLevels", true);
        EditorPrefs.SetBool("OneLife", true);
        EditorApplication.EnterPlaymode();
    }

    [Command]
    public static void PlayNormal()
    {
        EditorPrefs.DeleteKey("ShortLevels");
        EditorPrefs.DeleteKey("OneLife");
        EditorApplication.EnterPlaymode();
    }
    
    [Command]
    public static void UnlockNextLevel()
    {
        GameplayManager.UnlockNextLevel();
    }
    
    [Command]
    public static void ResetSaveData()
    {
        ApplicationLifetime.GetPlayerData().Money.Set(default);
        ApplicationLifetime.GetPlayerData().LastSelectedLevel.Set(default);
        ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(default);
    }

    [Command]
    public static void ReloadUI()
    {
        World.Get<MainUIController>().ReloadAll();
    }
    
    [Command]
    public static void SkipActivePhase()
    {
        switch (ApplicationLifetime.CurrentMode)
        {
            case TitleMenuMode titleMenuMode:
                GameplayManager.Fire(GlobalGameEvent.MainMenuGoNext);
                break;
            case PreFallSlideshowMode pre:
                GameplayManager.Fire(GlobalGameEvent.PreRunCutsceneFinished);
                break;
            case FallingGameActiveMode playing:
                // TODO no idea if this will skip correctly
                GameplayManager.Fire(GlobalGameEvent.PlatformerLevelEndReached);
                break;
            case PostFallLoseSlideshowMode lose:
            case PostFallWinSlideshowMode win:
                GameplayManager.Fire(GlobalGameEvent.PostRunCutsceneFinished);
                break;
            case LevelSelectMode levelSelect:
                break;
        }
        
    }
}