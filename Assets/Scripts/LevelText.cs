using System;

[Serializable]
public struct LevelText
{
    public string Name;
    public InGameText LevelStartText;
    public InGameText LevelBackgroundText;
    public InGameText LevelWinText;
    public InGameText LevelLoseText;
}