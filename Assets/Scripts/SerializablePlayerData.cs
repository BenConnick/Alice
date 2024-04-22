using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable player data backed by PlayerPrefs
/// </summary>
[Serializable]
public class SerializablePlayerData
{
    public IntWrapper Money;
    
    public EnumWrapper<LevelType> LastUnlockedLevel;
    
    public EnumWrapper<LevelType> LastSelectedLevel;

    protected readonly List<IPlayerDataValueWrapper> SerializableValues = new List<IPlayerDataValueWrapper>();

    public void SaveToDisk()
    {
        foreach (IPlayerDataValueWrapper val in SerializableValues)
        {
            PlayerPrefs.SetString(val.KeyName, val.Serialize());
        }
        PlayerPrefs.Save();
    }

    public bool TryLoadFromDisk()
    {
        InitializeWithDefaults();
        foreach (IPlayerDataValueWrapper serializable in SerializableValues)
        {
            if (PlayerPrefs.HasKey(serializable.KeyName))
            {
                serializable.Deserialize(PlayerPrefs.GetString(serializable.KeyName));
            }
        }
        return SerializableValues.Count > 0 && PlayerPrefs.HasKey(SerializableValues[0].KeyName);
    }

    private void InitializeWithDefaults()
    {
        LastUnlockedLevel = new EnumWrapper<LevelType>(LevelType.RabbitHole, nameof(LastUnlockedLevel));
        LastSelectedLevel = new EnumWrapper<LevelType>(LevelType.RabbitHole, nameof(LastUnlockedLevel));
        Money = new IntWrapper(0, nameof(Money));
        RegisterAllSerializedFields();
    }

    private void RegisterAllSerializedFields()
    {
        SerializableValues.Clear();
        SerializableValues.Add(LastUnlockedLevel);
        SerializableValues.Add(LastSelectedLevel);
        SerializableValues.Add(Money);
    }
}