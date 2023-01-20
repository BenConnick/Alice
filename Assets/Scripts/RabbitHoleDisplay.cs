using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RabbitHoleDisplay : MonoBehaviour
{
    public static List<RabbitHoleDisplay> All = new List<RabbitHoleDisplay>();

    public RabbitHoleGroup GameplayGroup;
    public Camera GameplayCamera => GameplayGroup.GameplayCam;
    public RabbitHole ObstacleContext => GameplayGroup.ObstacleContext;
    public RenderTexture AssociatedTexture => GameplayCamera != null ? GameplayCamera.targetTexture : null;
    public GameObject Overlay => GameplayGroup.UIOverlay.gameObject;
    public RenderTexture DefaultRT;

    [Header("UI")]
    [SerializeField] private RawImage rawImageComponent;

    // linked UI

    private TextMeshProUGUI scoreLabel => GameplayGroup.UIOverlay.ScoreLabel;
    private GameObject[] heartIcons => GameplayGroup.UIOverlay.HeartIcons;

    private bool invertedColor;

    private (int frame, Vector3 norm, Vector3 world) cachedCursorPosition = default;
    
    private void Awake()
    {
        if (!All.Contains(this)) All.Add(this);

        invertedColor = false; // rawImageComponent.material == invertedMaterial;
        if (ObstacleContext.OwnerLink == null) ObstacleContext.OwnerLink = this;

        // use new RT for each display
        if (GameplayCamera.targetTexture == DefaultRT)
        {
            SetRT(GetPooledRT());
        }
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
        All.Remove(this);
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
    [Obsolete]
    public RabbitHoleDisplay Create(Vector2 panelUIPos, Vector3 worldPos)
    {
        var source = this;
        RenderTexture newRT = GetPooledRT();
        Transform gameplayGroupCopy = Instantiate(source.GameplayCamera.transform.parent, source.GameplayCamera.transform.parent.parent);
        gameplayGroupCopy.transform.localPosition = worldPos; // shift over
        Camera cameraCopy = gameplayGroupCopy.GetComponentInChildren<Camera>();
        // cameraCopy.targetTexture = newRT; now covered below in "SetRT"

        var displayCopy = Instantiate(source,source.transform.parent);
        displayCopy.transform.localPosition = panelUIPos;
        displayCopy.GameplayGroup = gameplayGroupCopy.GetComponentInChildren<RabbitHoleGroup>();
        displayCopy.SetRT(newRT);
        displayCopy.ObstacleContext.OwnerLink = displayCopy;
        displayCopy.GetComponent<RawImage>().texture = newRT;
        return displayCopy;
    }

    private void SetRT(RenderTexture renderTexture)
    {
        if (rawImageComponent.texture != null && rawImageComponent.texture != DefaultRT)
        {
            if (rtTracker.Contains(rawImageComponent.texture as RenderTexture) && !freeRTPool.Contains(rawImageComponent.texture as RenderTexture))
            {
                Debug.LogWarning(gameObject.name + "(RabbitHoleDisplay): the render texture released by this display has not been returned to the pool and may remain in memory unless released.");
            }
        }
        GameplayCamera.targetTexture = renderTexture;
        rawImageComponent.texture = renderTexture;
    }

    // render texture pool is global, shared across all instances
    private static List<RenderTexture> freeRTPool = new List<RenderTexture>();
    private static List<RenderTexture> rtTracker = new List<RenderTexture>(); // track all created RTs, avoid leaks
    private RenderTexture GetPooledRT()
    {
        int RTW = DefaultRT.width;
        int RTH = DefaultRT.height;
        if (freeRTPool.Count > 0) {
            var ret = freeRTPool[0];
            freeRTPool.RemoveAt(0);
            return ret;
        }
        else
        {
            var ret = new RenderTexture(DefaultRT.descriptor);
            ret.filterMode = DefaultRT.filterMode;
            rtTracker.Add(ret);
            return ret;
        }
    }

    public Vector2 GetNormalizedCursorPos(Camera finalCam = null)
    {
        // only compute this once per frame
        if (Time.frameCount == cachedCursorPosition.frame)
            return cachedCursorPosition.norm;

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
        cachedCursorPosition = (Time.frameCount, posInViewport, GetCursorViewportWorldPos(posInViewport));

        // return
        return posInViewport;
    }

    public Vector3 GetCursorViewportWorldPos(Vector2 cursorViewportPos)
    {
        // only compute this once per frame
        if (Time.frameCount == cachedCursorPosition.frame)
            return cachedCursorPosition.world;

        // norm viewportCam pos to world* pos
        Vector3 gameplayPos = GameplayCamera.ViewportToWorldPoint(cursorViewportPos);
        // PerFrameVariableWatches.SetDebugQuantity("gameplayPos", gameplayPos.ToString());
        gameplayPos.z = ObstacleContext.transform.position.z; // z pos

        // cache
        cachedCursorPosition = (Time.frameCount, cachedCursorPosition.norm, gameplayPos);

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
        scoreLabel.text = "" + GM.Money; // + Util.CurrencyChar;
        // lives
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < ObstacleContext.VpLives);
        }
    }

    public void InvertColor(float duration)
    {
        SetColorInverted(true);
        Tween.Start(
            (t) => {
                // unused
            },
            duration,
            () => {
                SetColorInverted(false);
            });
    }
}
