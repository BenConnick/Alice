using System;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    public string name => LevelType.ToString(); // for inspector
    public LevelType LevelType;
    public float FallLength;
    public float TimeScaleMultiplier;
    public float FallSpeedMultiplier;
    public float FallAccelerationMultiplier;
    public ChunkSetAsset ChunkSet;
    public LevelTextAsset LevelText;
    public Sprite BackgroundSprite;
}