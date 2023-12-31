using System;
using System.Collections.Generic;

public static class CheatCodes
{
    // so-called "cheat codes"
    public static readonly IReadOnlyDictionary<LevelType, string> All = new Dictionary<LevelType, string>()
    {
        { LevelType.GardenOfChange, "DRINK" },
        { LevelType.CheshireDoors, "GROWN" },
        { LevelType.GardenOfSmoke, "GRINS" },
        { LevelType.MadTeaParty, "TWINS" },
    };

    public static void OnCheatCode(LevelType unlockCode)
    {
        LevelType newLevel = ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value;
        switch (ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value)
        {
            case LevelType.RabbitHole:
                if (unlockCode == LevelType.GardenOfChange)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.GardenOfChange:
                if (unlockCode == LevelType.CheshireDoors)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.CheshireDoors:
                if (unlockCode == LevelType.GardenOfSmoke)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.GardenOfSmoke:
                if (unlockCode == LevelType.MadTeaParty)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.MadTeaParty:
                // TBD
                break;
            default:
                break;
        }
        ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Set(newLevel);
    }
}
