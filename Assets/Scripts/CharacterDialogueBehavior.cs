using UnityEngine;

[RequireComponent(typeof(TypewriterText))]
public class CharacterDialogueBehavior : MonoBehaviour
{
    public static CharacterDialogueBehavior ActiveDialogue; // only one active at a time

    public string[] Lines;

    protected int index;

    private void OnEnable()
    {
        ActiveDialogue = this;
    }

    private void OnDisable()
    {
        if (ActiveDialogue == this) ActiveDialogue = null;
    }

    public virtual bool PlayNextLine()
    {
        if (index >= Lines.Length) return true;
        GetComponent<TypewriterText>().PlayTypewriter(Lines[index]);
        index++;
        return false;
    }
}
