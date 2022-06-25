using UnityEngine;

public class LevelChunk : MonoBehaviour
{
    public const float height = 6f;
    public int[] TopSlots;
    public int[] BottomSlots;
    public LevelCollider[] Obstacles => GetObstacles();

    private LevelCollider[] cachedObstacles;

    private LevelCollider[] GetObstacles()
    {
        if (cachedObstacles == null)
            cachedObstacles = GetComponentsInChildren<LevelCollider>();
        return cachedObstacles;
    }

#if UNITY_EDITOR
    public virtual void OnDrawGizmos()
    {
        // draw lanes
        Gizmos.color = Color.cyan;
        for (int i = 0; i <= LaneUtils.NumLanes; i++)
        {
            float laneX = LaneUtils.GetLaneCenterWorldPosition(i);
            Gizmos.DrawLine(new Vector3(laneX, transform.position.y - 3, 0), new Vector3(laneX, transform.position.y + 3, 0));
        }
    }
#endif
}
