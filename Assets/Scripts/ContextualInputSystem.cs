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
    
    public static bool AllowedOutside { get; set; }

    public static void Update()
    {
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
        if (AllowedOutside)
        {
            ActiveGameInstance = null;
            ViewNormalizedCursorPos =
                new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            ViewWorldCursorPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) +
                                 Vector3.forward * 10;
        }
        foreach (var instance in FallingGameInstance.All)
        {
            var viewport = instance.Viewport;
            Vector2 viewportCursor = viewport.GetNormalizedCursorPos(cam);
            if (Util.IsInBounds(viewportCursor))
            {
                ViewNormalizedCursorPos = viewportCursor;
                ActiveGameInstance = instance;
                break;
            }
        }

        if (ActiveGameInstance != null)
        {
            ViewWorldCursorPos = ActiveGameInstance.Viewport.GetCursorViewportWorldPos(ViewNormalizedCursorPos);
        }
        
        // PerFrameVariableWatches.SetDebugQuantity("mouse", Input.mousePosition.ToString());
    }
}
