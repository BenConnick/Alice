using UnityEngine;

[CreateAssetMenu]
public class LevelTextAsset : SerializableDataAsset<LevelText>
{
    public string Name => Data.Name;
}