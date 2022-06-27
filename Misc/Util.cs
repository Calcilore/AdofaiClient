using System;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace Adofai.Misc; 

public static class Util {
    public const float Pi = 3.141592653589793f;
    
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        return v.RotateRadians(MathHelper.ToRadians(degrees));
    }

    public static Vector2 RotateRadians(this Vector2 v, float radians) {
        float ca = (float)Math.Cos(radians);
        float sa = (float)Math.Sin(radians);
        return new Vector2(ca*v.X - sa*v.Y, sa*v.X + ca*v.Y);
    }

    public static Vector2 Lerp(Vector2 v, Vector2 v2, float by) {
        return new Vector2(
            MathHelper.Lerp(v.X, v2.X, by),
            MathHelper.Lerp(v.Y, v2.Y, by)
        );
    }

    public static Vector2 GetVector2FromJson(JsonElement json) {
            return new Vector2(json[0].GetSingle(), json[1].GetSingle());
    }
    
    // Adding and subtracting by non-vectors (sadly there are no operator overloading extension methods in C#)
    public static Vector2 Add(this Vector2 a, float b) {
        return new Vector2(a.X + b, a.Y + b);
    }
    
    public static Vector2 Sub(this Vector2 a, float b) {
        return new Vector2(a.X - b, a.Y - b);
    }
    
    public static Vector2 Mul(this Vector2 a, float b) {
        return new Vector2(a.X * b, a.Y * b);
    }
    
    public static Vector2 Div(this Vector2 a, float b) {
        return new Vector2(a.X / b, a.Y / b);
    }
    
    public static Point Add(this Point a, int b) {
        return new Point(a.X + b, a.Y + b);
    }
    
    public static Point Sub(this Point a, int b) {
        return new Point(a.X - b, a.Y - b);
    }
    
    public static Point Mul(this Point a, int b) {
        return new Point(a.X * b, a.Y * b);
    }
    
    public static Point Div(this Point a, int b) {
        return new Point(a.X / b, a.Y / b);
    }
}