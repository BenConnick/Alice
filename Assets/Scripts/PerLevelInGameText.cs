using System;

[Serializable]
public struct PerLevelInGameText
{
    public string Name;
    public InGameText LevelStartText;
    public InGameText LevelBackgroundText;
    public InGameText LevelWinText;
    public InGameText LevelLoseText;
}