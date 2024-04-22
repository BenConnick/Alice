using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// lane entities measure their
// x position in terms of discrete lane units
// and their
// y position in terms of floating units
public class LaneEntity : MonoBehaviour
{
    // public
    public int Lane = 0;
    public int WidthLanes => (int)FractionalWidthLanes;
    public float FractionalWidthLanes = 2;
    public float Height = 1f; // world-space units
    public byte Tags = Tag_DamageOnHit;

    public const byte Tag_DamageOnHit =     0b00000001;
    public const byte Tag_ShrinkOnHit =     0b00000010;
    public const byte Tag_GrowOnHit =       0b00000100;

    [Header("Editor Config")]
    public bool AutoLane = true;

    // y position is determined by the transform. In code, this is the center of the object in world-space, +Y is up, -Y is down
    public float Y => transform.position.y;

    public bool HasTag(byte tagToMatch)
    {
        return (Tags & tagToMatch) > 0;
    }

#if UNITY_EDITOR
    public virtual void OnDrawGizmos()
    {
        // update lane
        if (!Application.isPlaying && AutoLane)
        {
            var so = new SerializedObject(this);
            var laneProp = so.FindProperty("Lane");
            laneProp.intValue = LaneUtils.GetLanePosition(this);
            if (laneProp.intValue != Lane && so.hasModifiedProperties) so.ApplyModifiedProperties();
        }

        // get lane data
        Vector3 center = new Vector3(LaneUtils.GetWorldPosition(this), transform.position.y, transform.position.z);
        float w = LaneUtils.LaneScale * WidthLanes;
        Vector3 extents = new Vector3(w, Height, 0);

        // draw
        Color prev = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, extents);
        Gizmos.color = prev;

        // draw lanes
        Gizmos.color = Color.cyan;
        for (int i = 0; i <= LaneUtils.NumLanes; i++)
        {
            float laneX = LaneUtils.GetLaneCenterWorldPosition(i);
            Gizmos.DrawLine(new Vector3(laneX, transform.position.y - 10, 0), new Vector3(laneX, transform.position.y + 10, 0));
        }
    }
#endif
}
