using UnityEngine;

[CreateAssetMenu]
public class PerLevelInGameTextAsset : ScriptableObject
{
    public string Name => Data.Name;
    public PerLevelInGameText Data;
}