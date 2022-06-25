using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelCollider : MonoBehaviour
{
    public Collider2D Collider;
    public byte Tags = Tag_DamageOnHit;

    public const byte Tag_DamageOnHit = 0b00000001;
    public const byte Tag_ShrinkOnHit = 0b00000010;
    public const byte Tag_GrowOnHit = 0b00000100;

    public bool HasTag(byte tagToMatch)
    {
        return (Tags & tagToMatch) > 0;
    }

    public bool OverlapPoint(Vector3 point)
    {
        return Collider.OverlapPoint(point);
    }
}
