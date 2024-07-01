using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RabbitHoleDisplay : MonoBehaviour
{
    public RabbitHoleGroup GameplayGroup;
    public Camera GameplayCamera => GameplayGroup.GameplayCam;
    public RabbitHole ObstacleContext => GameplayGroup.ObstacleContext;
    public RenderTexture AssociatedTexture => GameplayCamera != null ? GameplayCamera.targetTexture : null;
    public RabbitHoleHUD Overlay => GameplayGroup.UIOverlay;
    public RenderTexture DefaultRT;

    [Header("UI")]
    [SerializeField] private RawImage rawImageComponent;

    public FallingGameInstance AssociatedGameInstance;

    // linked UI

    private TextMeshProUGUI scoreLabel => GameplayGroup.UIOverlay.ScoreLabel;
    private GameObject[] heartIcons => GameplayGroup.UIOverlay.HeartIcons;

    private bool invertedColor;

    private (int frame, Vector3 norm, Vector3 world) cachedCursorPosition = default;
    
    private void Awake()
    {
        if (AssociatedGameInstance == null)
        {
            AssociatedGameInstance = new FallingGameInstance(GameplayGroup, this);
        }

        invertedColor = false; // rawImageComponent.material == invertedMaterial;
        if (ObstacleContext.ViewportLink == null) ObstacleContext.ViewportLink = this;

        // use new RT for each display
        if (GameplayCamera.targetTexture == DefaultRT)
        {
            SetRT(GetPooledRT());
        }
    }

    private void Update()
    {
        AssociatedGameInstance.Tick();
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
        AssociatedGameInstance.OnDisplayDestroyed();
    }

    public void SetColorInverted(bool inverted)
    {
        if (invertedColor == inverted) return;
        //rawImageComponent.material = inverted ? invertedMaterial : defaultMaterial;
        InvertSpriteTintBehavior.SetAllInverted(inverted);
        invertedColor = inverted;
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
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void OnAppRestart()
    {
        freeRTPool.Clear();
        rtTracker.Clear();
    }
    #endif
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
            finalCam = World.Get<GameplayCameraBehavior>().GetComponent<Camera>();

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
        float progressTotal = AssociatedGameInstance.GetProgressTotal();
        
        // score
        AssociatedGameInstance.VpScore = Mathf.FloorToInt(progressTotal); // <- putting the actual score in the UI rendering is questionable at best...
        scoreLabel.text = string.Format($"{ApplicationLifetime.GetPlayerData().Money.Value:000000}"); // + Util.CurrencyChar;
        // lives
        for (int i = 0; i < heartIcons.Length; i++)
        {
            bool normalActive = i < AssociatedGameInstance.VpLives;
            bool flashing = invertedColor && i == AssociatedGameInstance.VpLives && Time.unscaledTime % .25f > .15f;
            heartIcons[i].SetActive(normalActive || flashing);
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
