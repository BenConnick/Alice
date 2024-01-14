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
        // obsolete
    }
}
