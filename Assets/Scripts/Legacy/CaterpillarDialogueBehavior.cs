using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarDialogueBehavior : CharacterDialogueBehavior
{
    // Serialized Fields
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
        List<WorldButton> inactiveButtons = new List<WorldButton> { CaterpillarButton, MushroomButton };
        inactiveButtons.Remove(target);
        activeButton.enabled = true;
        foreach (var btn in inactiveButtons) btn.Release();
        activeButton.Pulse();
    }
}
