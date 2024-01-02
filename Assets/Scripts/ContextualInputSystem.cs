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

    // where the cursor would be if it were in the world 
    // shown in the (raycast-hit) viewport
    public static Vector3 ViewWorldCursorPos { get; private set; }

    public static Vector3 ViewNormalizedCursorPos { get; private set; }

    public static bool UICapturedInput { get; set; }

    public static void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse up");
            ApplicationLifetime.CurrentMode.HandleInput(InputType.MouseUp);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse down");
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
        var cam = GlobalObjects.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();
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
