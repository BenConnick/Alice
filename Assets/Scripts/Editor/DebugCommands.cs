﻿using System.Collections;
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
    public static void SkipThis()
    {
        switch (ApplicationLifetime.CurrentMode)
        {
            case FallingGameActiveMode playing:
                // TODO no idea if this will skip correctly
                GameHelper.AllGameInstances(g => g.PlayOutroAnimation());
                break;
            case PostFallLoseCutsceneMode lose:
            case PostFallWinCutsceneMode win:
                Nav.Go(NavigationEvent.PostRunDialogueFinished);
                GameHelper.UnlockNextLevel();
                break;
            case LevelSelectMode levelSelect:
                break;
        }
        
    }
}