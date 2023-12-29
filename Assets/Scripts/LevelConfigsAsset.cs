using UnityEngine;

[CreateAssetMenu]
public class LevelConfigsAsset : ScriptableObject
{
    [SerializeField]
    public LevelConfig[] LevelConfigs;
}