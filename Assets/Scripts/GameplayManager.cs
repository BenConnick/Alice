using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for global gameplay state and functions
/// </summary>
/// <remarks>Companion class to <see cref="ApplicationLifetime"/></remarks>
public static class GameplayManager
{
    public static void ChangeSelectedLevel(LevelType newSelection)
    {
        SerializablePlayerData playerData = ApplicationLifetime.GetPlayerData();
        if (newSelection <= playerData.LastUnlockedLevel.Value)
        {
            playerData.LastSelectedLevel.Set(newSelection);
        }
    }

    public static void Fire(GlobalGameEvent gameEvent)
    {
        ApplicationLifetime.HandleGameEvent(gameEvent);
    }

    public static LevelConfig GetLevelConfig(LevelType levelType)
    {
        return GetLevelConfig(ToLevelIndex(levelType));
    }

    private static LevelConfig GetLevelConfig(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= MasterConfig.Values.LevelConfigs.Length)
        {
            Debug.LogWarning($"Level index '{levelIndex}' is out of range");
            return MasterConfig.Values.LevelConfigs[MissingLevelIndex];
        }
        return MasterConfig.Values.LevelConfigs[levelIndex];
    }

    public const int MissingLevelIndex = 7;

    public static bool IsLevelLocked(int levelIndex)
    {
        if (levelIndex == MissingLevelIndex)
        {
            return true;
        }
        return levelIndex > ToLevelIndex(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
    }

    public static bool IsLevelCompleted(int levelIndex)
    {
        if (levelIndex == MissingLevelIndex)
        {
            return false;
        }
        int highestUnlockedLevel = ToLevelIndex(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
        if (levelIndex == highestUnlockedLevel - 1)
        {
            return true;
        }

        return false;
    }

    public static LevelType FromLevelIndex(int index)
    {
        return GetLevelConfig(index).LevelType;
    }

    private static List<int> _LevelIndexCache = new List<int>(); // list index is enum value, value is config index

    public static int ToLevelIndex(LevelType levelType)
    {
        if (_LevelIndexCache.Count <= 0)
        {
            // populate cache
            for (int i = 0; i < MasterConfig.Values.LevelConfigs.Length; i++)
            {
                int enumValue = (int)MasterConfig.Values.LevelConfigs[i].LevelType;
                while (_LevelIndexCache.Count <= enumValue)
                {
                    _LevelIndexCache.Add(-1);
                }
                _LevelIndexCache[enumValue] = i;
            }
        }

        return _LevelIndexCache[(int)levelType];
    }

    public static void AllGameInstances(Action<FallingGameInstance> eachInstanceAction)
    {
        foreach (FallingGameInstance instance in FallingGameInstance.All)
        {
            eachInstanceAction(instance);
        }
    }

    public static LevelType HighestUnlockedLevel => ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
    public static LevelType SelectedLevel => LevelType.RabbitHole; // ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value;

    public static void UnlockNextLevel()
    {
        LevelType currentLevel = HighestUnlockedLevel;
        Debug.Log("Unlock level after " + HighestUnlockedLevel);
        LevelType nextLevel = FromLevelIndex(ToLevelIndex(currentLevel) + 1);
        ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(nextLevel);
        Debug.Log("New unlocked " + HighestUnlockedLevel);
        ApplicationLifetime.GetPlayerData().SaveToDisk();
    }

    public static LevelConfig GetCurrentLevelConfig()
    {
        LevelType levelIndex = ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value;
        return GetLevelConfig(levelIndex);
    }

    public static void AddMoney(int amount)
    {
        if (ContextualInputSystem.ActiveGameInstance != null)
        {
            ContextualInputSystem.ActiveGameInstance.VpScore.Coins += amount;
        }
        
        int plusOne = ApplicationLifetime.GetPlayerData().Money.Value + 1;
        ApplicationLifetime.GetPlayerData().Money.Set(plusOne);
        // TODO spawn collection celebration VFX
    }
}