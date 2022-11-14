using UnityEngine;

[RequireComponent(typeof(TypewriterText))]
public class CharacterDialogueBehavior : MonoBehaviour
{
    public string[] Lines;

    private int index;

    public bool PlayNextLine()
    {
        if (index >= Lines.Length) return true;
        GetComponent<TypewriterText>().PlayTypewriter(Lines[index]);
        index++;
        return false;
    }
}
