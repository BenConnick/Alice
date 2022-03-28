using UnityEngine;

// Math utilities for working with "Lanes"
public static class LaneUtils
{
    public static int NumLanes = 10;
    public static float LaneScale = 0.6f;

    public static float GetWorldPosition(LaneEntity entity, int? hypotheticalLanePosition = null)
    {
        int lane = hypotheticalLanePosition ?? entity.Lane;
        return (lane - NumLanes * .5f + entity.WidthLanes * .5f) * LaneScale;
    }

    public static int GetLanePosition(LaneEntity entity, float? hypotheticalXPosition = null)
    {
        float x = hypotheticalXPosition ?? entity.transform.position.x;
        return Mathf.RoundToInt(x / LaneScale + NumLanes * .5f - entity.WidthLanes * .5f);
    }

    public static bool CheckOverlap(LaneEntity a, LaneEntity b)
    {
        // lane
        if (a.Lane + a.WidthLanes <= b.Lane) return false; // A left of B
        if (b.Lane + b.WidthLanes <= a.Lane) return false; // B left of A

        // height
        if (a.Y - a.Height * .5f > b.Y + b.Height * .5f) return false; // A above B
        if (a.Y + a.Height * .5f < b.Y - b.Height * .5f) return false; // A below B

        // must be overlapping
        return true;
    }
}