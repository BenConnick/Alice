using UnityEngine;

[CreateAssetMenu]
public class IGT : ScriptableObject
{
    public static string EditorResourceName => ResourceName;
    private const string ResourceName = "IGT";
        
    private static IGT _Inst;
    
    [SerializeField]
    public PerLevelInGameTextAsset[] Levels;
        
    public static PerLevelInGameText ForLevel(int levelIndex)
    {
        LazyLoad();
        return _Inst.GetTextObjectForLevel(levelIndex);
    }

    private static void LazyLoad()
    {
        if (_Inst == null)
        {
            _Inst = Resources.Load<IGT>(ResourceName);
        }
    }

    private PerLevelInGameText GetTextObjectForLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= Levels.Length)
        {
            Debug.LogWarning("Failed to find text for level: " + levelIndex);
            return default;
        }

        return Levels[levelIndex].Data;
    }
}