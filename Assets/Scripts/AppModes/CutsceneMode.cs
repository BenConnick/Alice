using System;

public abstract class CutsceneMode : AppMode
{
    public event Action DialogueExhausted;
    
    protected InGameText _TextToShow;
    protected int _TextIndex;
    
    
    public CutsceneMode(StateMachine<AppMode> owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        // assign _TextToShow in subclass
        var ui = Root.Find<GameplayScreenBehavior>();
        ui.ShowStory();
        RequestDialogueAdvance();
    }

    public override void Tick(float dt)
    {
        
    }

    public override void OnExit()
    {
        var ui = Root.Find<GameplayScreenBehavior>();
        ui.HideStory();
    }

    public override bool HandleInput(ContextualInputSystem.InputType inputType)
    {
        if (inputType == ContextualInputSystem.InputType.MouseUp)
        {
            RequestDialogueAdvance();
        }
        return true;
    }

    protected void RequestDialogueAdvance()
    {
        var ui = Root.Find<GameplayScreenBehavior>();
        var StoryController = ui.StoryController;
        if (StoryController.IsStillTyping())
        {
            StoryController.FinishTyping();
        }
        else
        {
            if (_TextIndex >= _TextToShow.Lines.Length)
            {
                DialogueExhausted?.Invoke();
                return;
            }
            string line = _TextToShow.Lines[_TextIndex];
            StoryController.SetText(line, 10f);
            _TextIndex++;
        }
    }
}