using System.Collections.Generic;
using UnityEngine;

public class RabbitHoleDisplay : MonoBehaviour
{
    public Transform DisplayShape;
    public Camera GameplayCamera;
    public RabbitHole ObstacleContext;
    public Material AssociatedMaterial;
    public bool CanBeCopied = true;

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

    private void OnDestroy()
    {
        if (AssociatedMaterial != null
            && AssociatedMaterial.GetTexture("_MainTex") != null
            && AssociatedMaterial.GetTexture("_MainTex") is RenderTexture rt
            && !freeRTPool.Contains(rt))
        {
            freeRTPool.Add(rt);
        }
    }

    private void Start()
    {
        if (CanBeCopied)
        {
            Vector3 worldPos = new Vector3(100, 0, 0);
            Vector3 panelPos = new Vector3(2, 0, 20);
            Create(panelPos,worldPos);
            panelPos += PanelDefaultSpacing;
            worldPos += new Vector3(LevelInstanceSpacing,0,0);
            Create(panelPos, worldPos);
            panelPos += PanelDefaultSpacing;
            worldPos += new Vector3(LevelInstanceSpacing,0,0);
            Create(panelPos, worldPos);
        }
    }

    // assumes that there is a RabbitHoleDisplay instance
    // which exists in the scene and can be copied 
    public static RabbitHoleDisplay Create(Vector3 panelPos, Vector3 worldPos)
    {
        var source = GM.FindSingle<RabbitHoleDisplay>();
        RenderTexture newRT = GetPooledRT();
        Transform gameplayGroupCopy = Instantiate(source.GameplayCamera.transform.parent, source.GameplayCamera.transform.parent.parent);
        gameplayGroupCopy.transform.localPosition = worldPos; // shift over
        Camera cameraCopy = gameplayGroupCopy.GetComponentInChildren<Camera>();
        cameraCopy.targetTexture = newRT;
        Material matCopy = new Material(source.AssociatedMaterial);
        matCopy.name = "copy";
        matCopy.SetTexture("_MainTex", newRT);

        var copy = Instantiate(source,source.transform.parent);
        copy.transform.localPosition = panelPos;
        copy.AssociatedMaterial = matCopy;
        copy.GameplayCamera = cameraCopy;
        copy.CanBeCopied = false;
        copy.GetComponent<MeshRenderer>().sharedMaterial = matCopy;
        return copy;
    }

    const float LevelInstanceSpacing = 100f;
    static readonly Vector3 PanelDefaultSpacing = new Vector3(1,1,-1);
    const int RTW = 320;
    const int RTH = 320;
    private static List<RenderTexture> freeRTPool = new List<RenderTexture>();
    private static RenderTexture GetPooledRT()
    {
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
