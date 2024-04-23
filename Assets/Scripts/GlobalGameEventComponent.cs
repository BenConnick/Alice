using UnityEngine;
using UnityEngine.Serialization;

public class GlobalGameEventComponent : MonoBehaviour
{
    [FormerlySerializedAs("NavigationEvent")] public GlobalGameEvent GlobalGameEvent;

    public void FireEvent()
    {
        GameplayManager.Fire(GlobalGameEvent);
    }
}
