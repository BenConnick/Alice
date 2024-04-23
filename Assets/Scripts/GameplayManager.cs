using System;
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
        int levelIndex = ToLevelIndex(levelType);
        if (levelIndex < 0)
        {
            return default;
        }
        return MasterConfig.Values.LevelConfigs[levelIndex];
    }

    public static int ToLevelIndex(LevelType levelType)
    {
        for (int i = 0; i < MasterConfig.Values.LevelConfigs.Length; i++)
        {
            if (levelType == MasterConfig.Values.LevelConfigs[i].LevelType)
            {
                return i;
            }
        }

        Debug.LogWarning($"Level type '{levelType}' not found!");
        return -1;
    }

    public static void AllGameInstances(Action<FallingGameInstance> eachInstanceAction)
    {
        foreach (FallingGameInstance instance in FallingGameInstance.All)
        {
            eachInstanceAction(instance);
        }
    }

    public static LevelType HighestUnlockedLevel => ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
    public static LevelType SelectedLevel => ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value;

    public static void UnlockNextLevel()
    {
        LevelType currentLevel = HighestUnlockedLevel;
        LevelType nextLevel = currentLevel + 1;
        ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(nextLevel);
    }

    public static LevelConfig GetCurrentLevelConfig()
    {
        LevelType levelIndex = ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value;
        return GameplayManager.GetLevelConfig(levelIndex);
    }
}