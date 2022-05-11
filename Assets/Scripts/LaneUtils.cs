using UnityEngine;

// Math utilities for working with "Lanes"
public static class LaneUtils
{
    public static int NumLanes = 12;
    public static float LaneScale = .6f;

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

    private static float GetFractionalLanePosition(LaneCharacterMovement character, float? hypotheticalXPosition = null)
    {
        float x = hypotheticalXPosition ?? character.transform.position.x;
        return x / LaneScale + NumLanes * .5f - character.CharacterWidth * .5f;
    }

    public static float GetLaneCenterWorldPosition(int lane)
    {
        return (lane - NumLanes * .5f) * LaneScale;
    }

    public static bool CheckOverlap(LaneCharacterMovement player, LaneEntity collider)
    {
        // lane
        float fractionalLane = GetFractionalLanePosition(player);
        if (fractionalLane + player.CharacterWidth <= collider.Lane) return false; // A left of B
        if (collider.Lane + collider.WidthLanes <= fractionalLane) return false; // B left of A

        // height
        if (player.Y - player.Height * .5f > collider.Y + collider.Height * .5f) return false; // A above B
        if (player.Y + player.Height * .5f < collider.Y - collider.Height * .5f) return false; // A below B

        // must be overlapping
        return true;
    }
}