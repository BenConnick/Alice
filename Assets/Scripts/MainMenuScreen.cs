﻿using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Sprite[] levelSprites;
    public Image bg;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        bg.sprite = levelSprites[(int)GM.CurrentLevel];
        animator.SetTrigger("ResetAnimationTrigger");
    }

    // Update is called once per frame
    void Update()
    {
        // debug stuff
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            GM.OnDebugEvent(GM.DebugEvent.ShowNameEntryScreen);
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            GM.OnDebugEvent(GM.DebugEvent.SetLevelCaterpillar);
        }
    }

    public void OnStartPressed()
    {
        GM.OnGameEvent(GM.NavigationEvent.StartButton);
    }
}
