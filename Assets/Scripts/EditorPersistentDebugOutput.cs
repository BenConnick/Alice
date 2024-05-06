using System;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EditorPersistentDebugOutput
{
    public static Dictionary<string, string> MessageCache = new Dictionary<string, string>();

    public static Action MessagesUpdated;

    #if UNITY_EDITOR
    [InitializeOnEnterPlayMode]
    public static void ClearCache()
    {
        MessageCache.Clear();
        MessagesUpdated = null;
    }
    #endif

    [Conditional("UNITY_EDITOR")]
    public static void UpdateDebugMessage(string tag, string message)
    {
        MessageCache[tag] = message;
        MessagesUpdated?.Invoke();
    }
}