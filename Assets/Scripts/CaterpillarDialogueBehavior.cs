using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarDialogueBehavior : CharacterDialogueBehavior
{
    public override bool PlayNextLine()
    {
        if (index >= Lines.Length)
        {
            GM.FindSingle<SplitGameplayMomentAnimationController>().PlayBigMomentAnimPart1();
            return false;
        }
        GetComponent<TypewriterText>().PlayTypewriter(Lines[index]);
        index++;
        return false;
    }
}
