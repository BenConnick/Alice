using UnityEngine;

public class GameplayInnerDisplayCamera : MonoBehaviour
{
    public void LateUpdate()
    {
        var alice = GM.FindSingle<Alice>();
        float scale = alice.transform.localScale.x / Alice.DefaultScale;

        // match scale with the player and move accordingly
        var cam = GetComponent<Camera>();
        cam.orthographicSize = 3 * scale;

        //transform.localScale = new Vector3(12 / (float)alice.WidthLanes, 6 / (float)alice.WidthLanes, 1);
        Vector3 pos = alice.transform.position;
        if (GM.CurrentLevel == LevelType.Caterpillar)
            transform.localPosition = new Vector3(pos.x, pos.y - pos.y * scale, -10);
        else
            transform.localPosition = new Vector3(0, 0, -10);
    }
}
