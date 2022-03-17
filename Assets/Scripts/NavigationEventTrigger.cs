using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationEventTrigger : MonoBehaviour
{
    public GM.NavigationEvent NavigationEvent;

    public void FireEvent()
    {
        GM.OnGameEvent(NavigationEvent);
    }
}
