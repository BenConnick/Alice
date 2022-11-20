using UnityEngine;

public class SimpleTranslateBehavior : MonoBehaviour
{
    public Vector3 DistancePerSecond;

    public void Update()
    {
        //if (GM.IsGameplayPaused) return;
        transform.localPosition += Time.deltaTime * DistancePerSecond;
    }
}
