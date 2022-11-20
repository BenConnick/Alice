using System;
using UnityEngine;
public static class ContextualInputSystem
{
    public static RabbitHoleDisplay Context;

    // where the cursor would be if it were in the world 
    // shown in the (raycast-hit) viewport
    public static Vector3 ViewWorldCursorPos { get; private set; }

    public static bool UICapturedInput { get; set; }

    public static void Update()
    {
        if (UICapturedInput)
        {
            ViewWorldCursorPos = Vector3.zero;
        }
        else
        {
            ProcessInput();
        }
    }

    //private RaycastHit[] raycastHits = new RaycastHit[1];
    private static void ProcessInput()
    {
        // compare the mouse position against every display
        // PerFrameVariableWatches.SetDebugQuantity("mouse", Input.mousePosition.ToString());
        var cam = GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();
        foreach (var viewport in UnityEngine.Object.FindObjectsOfType<RabbitHoleDisplay>())
        {
            Vector2 normalizedCursorPos = viewport.GetNormalizedCursorPos(cam);
            if (Util.IsInBounds(normalizedCursorPos))
            {
                Context = viewport;
                ViewWorldCursorPos = viewport.GetCursorViewportWorldPos(normalizedCursorPos);
                break;
            }
        }
    }
}
