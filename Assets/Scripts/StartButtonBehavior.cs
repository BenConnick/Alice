using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonBehavior : MonoBehaviour
{
    public void OnPressed()
    {
        GM.OnGameEvent(GM.GameEvent.StartButton);
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            GM.OnGameEvent(GM.GameEvent.SkipIntroDialogue);
        } 
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            GM.OnGameEvent(GM.GameEvent.StartLevel);
        }
    }

#endif
}
