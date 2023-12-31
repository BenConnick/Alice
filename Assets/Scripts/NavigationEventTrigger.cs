using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationEventTrigger : MonoBehaviour
{
    public NavigationEvent NavigationEvent;

    public void FireEvent()
    {
        GameEventHandler.OnGameEvent(NavigationEvent);
    }
}
