using System;
using UnityEngine;

/// <summary>
/// A data container for all the edit-time designer config in the game.
/// </summary>
/// <remarks>
/// This is a master / central container<br/>
/// *Note: this is NOT a container for player data, nor other runtime generated values.<br/>
/// *For performance, config in the form of asset references can be made as weak refs (later).<br/>
/// *The only methods on this object are straightforward accessors. <br/>
/// *At runtime, most of these values will be readonly.
/// </remarks>
[Serializable]
public class MasterConfig : LazyLoadedGlobalResource<MasterConfig, MasterConfigAsset>
{
    /// <summary>
    /// Entry point to the runtime instance.
    /// </summary>
    public static MasterConfig Values => GetSingleton();

    public float BaseFallSpeed = 6;

    public float BaseFallAcceleration = 0.01f;
    
    public LevelConfig[] LevelConfigs;
    
    // TODO animation configs
    
    [Header("Animation")] 
    public float titleAnimationDistance;
    public float titleAnimationDuration;
    public float introAnimationDistance;
    public float outroAnimationDistance;
    public float introAnimationSpeed;
    public AnimationCurve titleAnimationCurve;
    public AnimationCurve introAnimationCurve;

    public LevelConfig GetLevelConfig(LevelType levelType)
    {
        int levelIndex = GameHelper.ToIndex(levelType);
        if (levelIndex < 0)
        {
            return default;
        }
        return LevelConfigs[levelIndex];
    }
}