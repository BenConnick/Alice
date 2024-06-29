using System;
using UnityEngine;
public static class ContextualInputSystem
{
    public enum InputType
    {
        Error,
        MouseUp,
        MouseDown
    }
    
    public static RabbitHoleDisplay ActiveViewport => ActiveGameInstance?.Viewport;
    
    public static FallingGameInstance ActiveGameInstance;
    
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void EditorReload()
    {
        ActiveGameInstance = null;
    }
    #endif

    // where the cursor would be if it were in the world 
    // shown in the (raycast-hit) viewport
    public static Vector3 ViewWorldCursorPos { get; private set; }

    public static Vector3 ViewNormalizedCursorPos { get; private set; }

    public static bool UICapturedInput { get; set; }

    public static void Update()
    {
        if (ReferenceEquals(ActiveGameInstance, null) && FallingGameInstance.All.Count > 0)
        {
            ActiveGameInstance = FallingGameInstance.All[0];
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            ApplicationLifetime.CurrentMode.HandleInput(InputType.MouseUp);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ApplicationLifetime.CurrentMode.HandleInput(InputType.MouseDown);
        }

        if (UICapturedInput)
        {
            ViewWorldCursorPos = Vector3.zero;
        }
        else
        {
            UpdateActiveInstance();
        }
    }

    //private RaycastHit[] raycastHits = new RaycastHit[1];
    private static void UpdateActiveInstance()
    {
        // compare the mouse position against every display
        var cam = World.Get<GameplayCameraBehavior>().GetComponent<Camera>();
        foreach (var instance in FallingGameInstance.All)
        {
            var viewport = instance.Viewport;
            ViewNormalizedCursorPos = viewport.GetNormalizedCursorPos(cam);
            if (Util.IsInBounds(ViewNormalizedCursorPos))
            {
                ActiveGameInstance = instance;
                ViewWorldCursorPos = viewport.GetCursorViewportWorldPos(ViewNormalizedCursorPos);
                break;
            }
        }
        
        // PerFrameVariableWatches.SetDebugQuantity("mouse", Input.mousePosition.ToString());
    }
}
