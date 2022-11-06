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
        EditorApplication.EnterPlaymode();
    }

    [Command]
    public static void PlayNormal()
    {
        EditorPrefs.DeleteKey("ShortLevels");
        EditorApplication.EnterPlaymode();
    }
}