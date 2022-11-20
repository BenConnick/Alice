using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class InvertSpriteTintBehavior : MonoBehaviour
{
    private static readonly List<InvertSpriteTintBehavior> allInstances = new List<InvertSpriteTintBehavior>();

    public static void SetAllInverted(bool inverted)
    {
        foreach (var b in allInstances) b.SetInverted(inverted);
    }
    
    private Color originalColor;
    private Color invertedColor;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        allInstances.Add(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        invertedColor = new Color(1 - originalColor.r, 1 - originalColor.g, 1 - originalColor.b, 1);
    }

    private void OnDestroy()
    {
        allInstances.Remove(this);
    }

    private void SetInverted(bool inverted)
    {
        spriteRenderer.color = inverted ? invertedColor : originalColor;
    }
}
