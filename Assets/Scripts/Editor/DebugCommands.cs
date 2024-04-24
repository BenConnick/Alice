using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// ReSharper disable UnusedMember.Global

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
    public static void ClearSaveData()
    {
        ApplicationLifetime.GetPlayerData().ResetValuesToDefaults();
        ApplicationLifetime.GetPlayerData().SaveToDisk();
        UIHelper.ShowToast("Save Data Reset To Defaults");
    }

    [Command]
    public static void ReloadUI()
    {
        World.Get<MainUIController>().ReloadAll();
    }

    [Command]
    public static void JumpToLevelSelect()
    {
        // skip title animation
        if (ApplicationLifetime.CurrentMode is TitleMenuMode)
        {
            SkipActivePhase();
        }

        // skip intro cutscene
        if (ApplicationLifetime.CurrentMode is PreFallSlideshowMode)
        {
            SkipActivePhase(); 
        }

        // skip gameplay
        SkipActivePhase();
        
        // skip post-gameplay slideshow
        SkipActivePhase();
        
        UIHelper.ShowToast("Skipping...");
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
                GameplayManager.Fire(GlobalGameEvent.PlatformerLevelEndAnimationFinished);
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