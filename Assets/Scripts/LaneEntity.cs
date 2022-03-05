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
    public float Height = 1f;
    public float Y; // this is the center of the object in world-space, +Y is up, -Y is down
}
