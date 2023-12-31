using System;
using UnityEngine;
public static class ContextualInputSystem
{
    public static RabbitHoleDisplay GameplayContext;

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
            if (ApplicationLifetime.CurrentMode == ApplicationLifetime.GameMode.PreMainMenu)
            {
                var rh = GlobalObjects.FindSingle<RabbitHoleDisplay>();
                rh.GameplayGroup.ObstacleContext.FastForwardTitleIntro();
            }
            else if (ApplicationLifetime.CurrentMode == ApplicationLifetime.GameMode.MainMenu)
            {
                Debug.Log("Title Mouse up");
                GameEventHandler.OnGameEvent(NavigationEvent.MainMenuGoNext);
            }
            else if (ApplicationLifetime.CurrentMode == ApplicationLifetime.GameMode.GameOver)
            {
                Debug.Log("Game over Mouse up");
                GameEventHandler.OnGameEvent(NavigationEvent.GameOverGoNext);
            }
        }

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
        var cam = GlobalObjects.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();
        foreach (var viewport in RabbitHoleDisplay.All)
        {
            ViewNormalizedCursorPos = viewport.GetNormalizedCursorPos(cam);
            if (Util.IsInBounds(ViewNormalizedCursorPos))
            {
                GameplayContext = viewport;
                ViewWorldCursorPos = viewport.GetCursorViewportWorldPos(ViewNormalizedCursorPos);
                break;
            }
        }
    }
}
