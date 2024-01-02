using System.Collections.Generic;

public abstract class AppMode : StateMachine<AppMode>.State
{
    public static readonly List<AppMode> All = new List<AppMode>();

    public abstract string Name { get; }

    protected AppMode(StateMachine<AppMode> owner) : base(owner)
    {
        All.Add(this);
    }

    /// <summary>
    /// Handle delegated input events based on the current game mode
    /// </summary>
    /// <param name="inputType">A limited set of input types</param>
    /// <returns>True if handler should consume the input</returns>
    public abstract bool HandleInput(ContextualInputSystem.InputType inputType);

    ~AppMode()
    {
        All.Remove(this);
    }
}