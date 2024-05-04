using System;
using System.Collections.Generic;
using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    [Obsolete]
    [Header("Config")] public ObstacleSpawnersConfig SpawnersConfig;

    [Header("Children")] public SpriteRenderer[] BackgroundTiles;

    [Header("Animation")] public float titleAnimationDistance;
    public MenuGraphics menuGraphics;

    [Header("Assets")] [SerializeField] public Sprite fallbackBackgroundSprite;

    // non-serialized fields
    public FallingGameInstance AssociatedGameInstance;
    public RabbitHoleDisplay ViewportLink { get; set; }
}