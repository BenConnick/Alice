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
}