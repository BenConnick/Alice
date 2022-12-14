using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RabbitHoleDisplay : MonoBehaviour
{
    public Camera GameplayCamera;
    public RabbitHole ObstacleContext;
    public RenderTexture AssociatedTexture => GameplayCamera != null ? GameplayCamera.targetTexture : null;
    public GameObject Overlay;


    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private GameObject[] heartIcons;
    [SerializeField] private RawImage rawImageComponent;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material invertedMaterial;

    private bool invertedColor;

    private (int frame, Vector3 norm, Vector3 world) cachedCursorPositions = default;
    
    private void Awake()
    {
        invertedColor = false; // rawImageComponent.material == invertedMaterial;
        if (ObstacleContext.OwnerLink == null) ObstacleContext.OwnerLink = this;
    }

    private void LateUpdate()
    {
        // update UI
        UpdateGameplayUI();
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

    public void SetColorInverted(bool inverted)
    {
        if (invertedColor == inverted) return;
        //rawImageComponent.material = inverted ? invertedMaterial : defaultMaterial;
        InvertSpriteTintBehavior.SetAllInverted(inverted);
        invertedColor = inverted;
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

    public Vector2 GetNormalizedCursorPos(Camera finalCam = null)
    {
        // only compute this once per frame
        if (Time.frameCount == cachedCursorPositions.frame)
            return cachedCursorPositions.norm;

        // fallback to default
        if (finalCam == null)
            finalCam = GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();

        // mouse pos
        // PerFrameVariableWatches.SetDebugQuantity("mouse", finalCam.ScreenToViewportPoint(Input.mousePosition).ToString());
        // rect pos
        var viewportUI = this;
        var rt = viewportUI.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, finalCam, out Vector2 localPos);
        // rect pos to norm viewportCam pos
        Vector2 posInViewport = new Vector2((localPos.x + rt.rect.width * .5f) / (rt.rect.width), (localPos.y + rt.rect.height * .5f) / (rt.rect.height));
        // PerFrameVariableWatches.SetDebugQuantity("posInViewport", posInViewport.ToString());

        // cache
        cachedCursorPositions = (Time.frameCount, posInViewport, GetCursorViewportWorldPos(posInViewport));

        // return
        return posInViewport;
    }

    public Vector3 GetCursorViewportWorldPos(Vector2 cursorViewportPos)
    {
        // only compute this once per frame
        if (Time.frameCount == cachedCursorPositions.frame)
            return cachedCursorPositions.world;

        // norm viewportCam pos to world* pos
        Vector3 gameplayPos = GameplayCamera.ViewportToWorldPoint(cursorViewportPos);
        // PerFrameVariableWatches.SetDebugQuantity("gameplayPos", gameplayPos.ToString());
        gameplayPos.z = ObstacleContext.transform.position.z; // z pos

        // cache
        cachedCursorPositions = (Time.frameCount, cachedCursorPositions.norm, gameplayPos);

        return gameplayPos;
    }

    public Vector3 GetCursorWorldPos()
    {
        return GetCursorViewportWorldPos(GetNormalizedCursorPos());
    }

    private void UpdateGameplayUI()
    {
        float progressTotal = ObstacleContext.transform.localPosition.y - ObstacleContext.InitialHeight;
        
        // score
        ObstacleContext.VpScore = Mathf.FloorToInt(progressTotal); // <- putting the actual score in the UI rendering is questionable at best...
        scoreLabel.text = "" + GM.Money + Util.CurrencyChar;
        // lives
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < ObstacleContext.VpLives);
        }
    }

    public void InvertForTime(float time)
    {
        Tween.Start(
            (t) => {
                SetColorInverted(true);
            },
            time,
            () => {
                SetColorInverted(false);
            });
    }
}
