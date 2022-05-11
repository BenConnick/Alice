using UnityEngine;

public class GameplayInnerDisplayCamera : MonoBehaviour
{
    public const float DefaultOrthoSize = 4;

    public void LateUpdate()
    {
        var alice = GM.FindSingle<Alice>();
        float scale = alice.transform.localScale.x / Alice.DefaultScale;

        // match scale with the player and move accordingly
        var cam = GetComponent<Camera>();
        cam.orthographicSize = DefaultOrthoSize * scale;

        //transform.localScale = new Vector3(12 / (float)alice.WidthLanes, 6 / (float)alice.WidthLanes, 1);
        Vector3 pos = alice.transform.position;
        transform.localPosition = new Vector3(0, pos.y - pos.y * scale, -10);
    }
}
