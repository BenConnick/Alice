using System;
using UnityEngine;

public class GameHelper
{
    public static int ToIndex(LevelType levelType)
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

    public static LevelType GetHighestUnlockedLevel()
    {
        LevelType currentLevel = ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
        return currentLevel;
    }

    public static void UnlockNextLevel()
    {
        LevelType currentLevel = GetHighestUnlockedLevel();
        LevelType nextLevel = currentLevel + 1;
        ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(nextLevel);
    }
    
    public static LevelConfig GetCurrentLevelConfig()
    {
        LevelType levelIndex = ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value;
        return GetLevelConfig(levelIndex);
    }
    
    public static LevelConfig GetLevelConfig(LevelType levelType)
    {
        int levelIndex = ToIndex(levelType);
        if (levelIndex < 0)
        {
            return default;
        }
        return MasterConfig.Values.LevelConfigs[levelIndex];
    }
}