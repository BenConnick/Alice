using System;
using System.Collections.Generic;

public static class CheatCodes
{
    // so-called "cheat codes"
    public static readonly IReadOnlyDictionary<LevelType, string> All = new Dictionary<LevelType, string>()
    {
        { LevelType.Caterpillar, "DRINK" },
        { LevelType.CheshireCat, "GROWN" },
        { LevelType.MadHatter, "GRINS" },
        { LevelType.QueenOfHearts, "TWINS" },
    };

    public static void OnCheatCode(LevelType unlockCode)
    {
        LevelType newLevel = GM.CurrentLevel;
        switch (GM.CurrentLevel)
        {
            case LevelType.RabbitHole:
                if (unlockCode == LevelType.Caterpillar)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.Caterpillar:
                if (unlockCode == LevelType.CheshireCat)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.CheshireCat:
                if (unlockCode == LevelType.MadHatter)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.MadHatter:
                if (unlockCode == LevelType.QueenOfHearts)
                {
                    newLevel = unlockCode;
                }
                break;
            case LevelType.QueenOfHearts:
                // TBD
                break;
            default:
                break;
        }
        GM.CurrentLevel = newLevel;
    }
}
