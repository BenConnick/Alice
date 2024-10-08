﻿using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashingBehavior : MonoBehaviour
{
    public float flashOffTime = 0.2f;

    private SpriteRenderer spriteRenderer;
    private float flashingTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flashingTimer > 0 && Time.timeScale > 0)
        {
            spriteRenderer.enabled = flashingTimer % flashOffTime > flashOffTime * .5f;
            flashingTimer -= Time.deltaTime;
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }

    public void StartFlashing(float duration=2f)
    {
        flashingTimer = duration;
    }

    public void StopFlashing()
    {
        flashingTimer = 0;
    }

    public bool IsFlashing()
    {
        return flashingTimer > 0;
    }
}
