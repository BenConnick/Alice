using UnityEngine;

public class RabbitHoleDisplay : MonoBehaviour
{
    public Transform DisplayShape;
    public Camera GameplayCamera;
    public RabbitHole ObstacleContext;
    public Material AssociatedMaterial;

    public int GetLane(float worldX)
    {
        // (quad has dimensions of 1x1, so we can use the scale to get the size)
        float offset = worldX - ObstacleContext.transform.position.x;
        return LaneUtils.GetLanePosition(offset);
    }

    public float GetLaneCenterWorldPos(int lane)
    {
        return LaneUtils.GetWorldPosition(lane) + ObstacleContext.transform.position.x;
    }
}
