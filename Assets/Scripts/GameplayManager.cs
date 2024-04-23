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
}