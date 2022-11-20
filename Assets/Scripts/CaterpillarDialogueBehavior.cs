using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarDialogueBehavior : CharacterDialogueBehavior
{
    // Serialized Fields
    public WorldButton AliceButton;
    public WorldButton CaterpillarButton;
    public WorldButton MushroomButton;
    public GameObject CaterpillarVanishParticles;

    public override bool PlayNextLine()
    {
        if (index >= Lines.Length)
        {
            MushroomButton.Release();
            return true;
        }
        switch (index)
        {
            case 0:
                RequireClickCharacter(CaterpillarButton);
                break;
            case 1:
            case 2:
                RequireClickCharacter(AliceButton);
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                RequireClickCharacter(CaterpillarButton);
                break;
            case 7:
                RequireClickCharacter(AliceButton);
                break;
            case 8:
                RequireClickCharacter(CaterpillarButton);
                break;
            case 9:
                PlayCaterpillarDisappear();
                RequireClickCharacter(MushroomButton);
                break;
        }
        GetComponent<TypewriterText>().PlayTypewriter(Lines[index]);
        index++;
        return false;
    }

    private void PlayCaterpillarDisappear()
    {
        CaterpillarButton.gameObject.SetActive(false);
        CaterpillarVanishParticles.SetActive(true);
    }

    private void RequireClickCharacter(WorldButton target)
    {
        WorldButton activeButton = target;
        List<WorldButton> inactiveButtons = new List<WorldButton> { AliceButton, CaterpillarButton, MushroomButton };
        inactiveButtons.Remove(target);
        activeButton.enabled = true;
        foreach (var btn in inactiveButtons) btn.Release();
        activeButton.Pulse();
    }
}
