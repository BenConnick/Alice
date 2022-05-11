using UnityEngine;

public class GameplayDisplayQuad : MonoBehaviour
{
    public void Update()
    {
        var alice = GM.FindSingle<Alice>();
        int scale = alice.WidthLanes;

        // match scale with the player and move accordingly
        var cam = GetComponent<Camera>();
        cam.orthographicSize = 3 * scale;

        //transform.localScale = new Vector3(12 / (float)alice.WidthLanes, 6 / (float)alice.WidthLanes, 1);
        transform.localPosition = new Vector3(scale < 2 ? alice.transform.position.x*.5f : 0, 0, 0);
    }
}
