using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BWTexData", menuName = "ScriptableObjects/BWTexData")]
public class BWTexData : ScriptableObject
{
    [Serializable]
    public struct BWTexDataSingle
    {
        public Sprite Original;
        public int Width;
        public PositionsByColor[] PixelData; // kind of a minsnomer now that it supports color
    }

    [Serializable]
    public struct PositionsByColor
    {
        public Color Color;
        public int2[] Positions;
    }

    [Serializable]
    public struct int2
    {
        public int x;
        public int y;
    }

    [SerializeField] public BWTexDataSingle[] Images;

}
