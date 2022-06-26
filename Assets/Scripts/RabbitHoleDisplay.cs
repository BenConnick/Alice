using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RabbitHoleDisplay : MonoBehaviour
{
    public Camera GameplayCamera;
    public RabbitHole ObstacleContext;
    public RenderTexture AssociatedTexture => GameplayCamera != null ? GameplayCamera.targetTexture : null;

    public void Awake()
    {
        if (ObstacleContext.OwnerLink == null) ObstacleContext.OwnerLink = this;
    }

    public int GetLane(float worldX)
    {
        float offset = worldX - ObstacleContext.transform.position.x;
        return LaneUtils.GetLanePosition(offset);
    }

    public float GetLaneCenterWorldPos(int lane)
    {
        return LaneUtils.GetWorldPosition(lane) + ObstacleContext.transform.position.x;
    }

    private void OnDestroy()
    {
        if (AssociatedTexture != null && !freeRTPool.Contains(AssociatedTexture))
        {
            freeRTPool.Add(AssociatedTexture);
        }
    }

    // assumes that there is a RabbitHoleDisplay instance
    // which exists in the scene and can be copied 
    public static RabbitHoleDisplay Create(Vector2 panelUIPos, Vector3 worldPos)
    {
        var source = GM.GameplayScreen.GetComponentInChildren<RabbitHoleDisplay>();
        RenderTexture newRT = GetPooledRT();
        Transform gameplayGroupCopy = Instantiate(source.GameplayCamera.transform.parent, source.GameplayCamera.transform.parent.parent);
        gameplayGroupCopy.transform.localPosition = worldPos; // shift over
        Camera cameraCopy = gameplayGroupCopy.GetComponentInChildren<Camera>();
        cameraCopy.targetTexture = newRT;

        var copy = Instantiate(source,source.transform.parent);
        copy.transform.localPosition = panelUIPos;
        copy.ObstacleContext = gameplayGroupCopy.GetComponentInChildren<RabbitHole>();
        copy.ObstacleContext.OwnerLink = copy;
        copy.GameplayCamera = cameraCopy;
        copy.GetComponent<RawImage>().texture = newRT;
        return copy;
    }

    // render texture pool is global, shared across all instances
    private static List<RenderTexture> freeRTPool = new List<RenderTexture>();
    private static RenderTexture GetPooledRT()
    {
        const int RTW = 320;
        const int RTH = 320;
        if (freeRTPool.Count > 0) {
            var ret = freeRTPool[0];
            freeRTPool.RemoveAt(0);
            return ret;
        }
        else
        {
            return new RenderTexture(new RenderTextureDescriptor(RTW,RTH,RenderTextureFormat.Default));
        }
    }
}
