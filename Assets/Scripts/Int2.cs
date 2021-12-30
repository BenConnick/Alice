using UnityEngine;

public struct Int2
{
    public Int2(int xValue, int yValue)
    {
        x = xValue;
        y = yValue;
    }

    public int x;
    public int y;

    public static Vector2 operator *(Int2 int2, float scalar)
    {
        return new Vector2(int2.x * scalar, int2.y * scalar);
    }

    public static Vector2 operator *(float scalar, Int2 int2)
    {
        return new Vector2(int2.x * scalar, int2.y * scalar);
    }

    public static Vector2 operator -(Vector2 vec2, Int2 int2)
    {
        return new Vector2(vec2.x - int2.x, vec2.y - int2.y);
    }

    public static Vector2 operator -(Int2 int2, Vector2 vec2)
    {
        return new Vector2(int2.x - vec2.x, int2.y - vec2.y);
    }

    public static Vector2 operator +(Vector2 vec2, Int2 int2)
    {
        return new Vector2(vec2.x + int2.x, vec2.y + int2.y);
    }

    public static Vector2 operator +(Int2 int2, Vector2 vec2)
    {
        return new Vector2(int2.x + vec2.x, int2.y + vec2.y);
    }

    public static Int2 operator +(Int2 left, Int2 right)
    {
        return new Int2(left.x + right.x, left.y + right.y);
    }

    public static Int2 operator -(Int2 left, Int2 right)
    {
        return new Int2(left.x - right.x, left.y - right.y);
    }
}