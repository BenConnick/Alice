using UnityEngine;

[RequireComponent(typeof(TypewriterText))]
public class CharacterDialogueBehavior : MonoBehaviour
{
    public string[] Lines;

    protected int index;

    public virtual bool PlayNextLine()
    {
        if (index >= Lines.Length) return true;
        GetComponent<TypewriterText>().PlayTypewriter(Lines[index]);
        index++;
        return false;
    }
}
