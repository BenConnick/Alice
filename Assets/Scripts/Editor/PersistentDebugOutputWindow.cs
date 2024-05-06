using System;
using UnityEditor;
using UnityEngine;

public class PersistentDebugOutputWindow : EditorWindow
{
    [MenuItem("Window/Editor Debug Messages")]
    public static void ShowWindow()
    {
        PersistentDebugOutputWindow prompt = (PersistentDebugOutputWindow)GetWindow(typeof(PersistentDebugOutputWindow));
        prompt.Show();
    }

    private bool initialized;
    private Vector2 prevScrollPosition;

    public void OnGUI()
    {
        Initialize();
        ListenForUpdates();
        
        // early return: nothing to show
        if (EditorPersistentDebugOutput.MessageCache.Count == 0)
        {
            GUILayout.Label("[Nothing to show]");
            return;
        }

        // scrolling list of all 'messages'
        using (new EditorGUILayout.ScrollViewScope(prevScrollPosition))
        {
            foreach (var kv in EditorPersistentDebugOutput.MessageCache)
            {
                GUILayout.Label($"[{kv.Key}] {kv.Value}");
            }
        }
    }

    private void Initialize()
    {
        if (initialized) return;
        minSize = new Vector2(10, 30);
        initialized = true;
    }

    private void ListenForUpdates()
    {
        if (EditorPersistentDebugOutput.MessagesUpdated == null || EditorPersistentDebugOutput.MessagesUpdated.GetInvocationList().Length == 0)
        {
            EditorPersistentDebugOutput.MessagesUpdated += Repaint;
        }
    }
}