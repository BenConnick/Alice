using UnityEngine;

// lane entities measure their
// x position in terms of discrete lane units
// and their
// y position in terms of floating units
public class LaneEntity : MonoBehaviour
{
    // public
    public int Lane = 0;
    public int WidthLanes = 2;
    public float Height = 1f; // world-space units

    // y position is determined by the transform. In code, this is the center of the object in world-space, +Y is up, -Y is down
    public float Y => transform.position.y;

    public void OnDrawGizmos()
    {
        Vector3 center = new Vector3(LaneUtils.GetWorldPosition(this), transform.position.y, transform.position.z);
        float w = LaneUtils.LaneScale * WidthLanes;
        Vector3 extents = new Vector3(w, Height, 0);

        // draw
        Color prev = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, extents);
        Gizmos.color = prev;
    }
}
