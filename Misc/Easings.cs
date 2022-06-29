using System;
using Microsoft.Xna.Framework;

namespace Adofai.Misc; 

// This class is a mess.
public static class Easings {
    // Convert ADOFAI "Json" to C# enum.
    public static Ease AdofaiEasingToEnum(string ado) {
        if (!Enum.TryParse(ado, true, out Ease e)) {
            throw new Exception($"Easing {ado} is an invalid easing");
        }

        return e;
    }

    /// <summary>
    /// Eases from 0 to 1.
    /// </summary>
    /// <param name="ease">The easing method to use</param>
    /// <param name="t">The value to ease</param>
    /// <returns>The eased Value</returns>
    public static float DoEase(Ease ease, float t) {
        switch (ease) {
            case Ease.Linear:
                return t;
            
            case Ease.InSine:
                return (float)-Math.Cos(t * Math.PI / 2) + 1;
            case Ease.OutSine:
                return (float)Math.Sin(t * Math.PI / 2);
            case Ease.InOutSine:
                return (float)(Math.Cos(t * Math.PI) - 1) / -2;

            case Ease.InQuad:
                return InQuad(t);
            case Ease.OutQuad:
                return OutQuad(t);
            case Ease.InOutQuad:
                if (t < 0.5) return InQuad(t * 2) / 2;
                return 1 - InQuad((1 - t) * 2) / 2;
                
            case Ease.InCubic:
                return InCubic(t);
            case Ease.OutCubic:
                return OutCubic(t);
            case Ease.InOutCubic:
                if (t < 0.5) return InCubic(t * 2) / 2;
                return 1 - InCubic((1 - t) * 2) / 2;
                
            case Ease.InQuart:
                return InQuart(t);
            case Ease.OutQuart:
                return OutQuart(t);
            case Ease.InOutQuart:
                if (t < 0.5) return InQuart(t * 2) / 2;
                return 1 - InQuart((1 - t) * 2) / 2;
                
            case Ease.InQuint:
                return InQuint(t);
            case Ease.OutQuint:
                return OutQuint(t);
            case Ease.InOutQuint:
                if (t < 0.5) return InQuint(t * 2) / 2;
                return 1 - InQuint((1 - t) * 2) / 2;
                
            case Ease.InExpo:
                return InExpo(t);
            case Ease.OutExpo:
                return OutExpo(t);
            case Ease.InOutExpo:
                if (t < 0.5) return InExpo(t * 2) / 2;
                return 1 - InExpo((1 - t) * 2) / 2;
                
            case Ease.InCirc:
                return InCirc(t);
            case Ease.OutCirc:
                return OutCirc(t);
            case Ease.InOutCirc:
                if (t < 0.5) return InCirc(t * 2) / 2;
                return 1 - InCirc((1 - t) * 2) / 2;
                
            case Ease.InElastic:
                return InElastic(t);
            case Ease.OutElastic:
                return OutElastic(t);
            case Ease.InOutElastic:
                if (t < 0.5) return InElastic(t * 2) / 2;
                return 1 - InElastic((1 - t) * 2) / 2;
                
            case Ease.InBack:
                return InBack(t);
            case Ease.OutBack:
                return OutBack(t);
            case Ease.InOutBack:
                if (t < 0.5) return InBack(t * 2) / 2;
                return 1 - InBack((1 - t) * 2) / 2;
                
            case Ease.InBounce:
                return InBounce(t);
            case Ease.OutBounce:
                return OutBounce(t);
            case Ease.InOutBounce:
                if (t < 0.5) return InBounce(t * 2) / 2;
                return 1 - InBounce((1 - t) * 2) / 2;
                
            case Ease.Flash:
                return InFlash(t);
            case Ease.InFlash:
                return InFlash(t);
            case Ease.OutFlash:
                return OutFlash(t);
            case Ease.InOutFlash:
                if (t < 0.5) return InFlash(t * 2) / 2;
                return 1 - InFlash((1 - t) * 2) / 2;
        }
        
        throw new ArgumentOutOfRangeException(nameof(ease), ease, null);
    }
    
    public static float DoEase(Ease ease, float a, float b, float by) {
        // Get ease for 0-1, then expand that to desired numbers with lerp
        return MathHelper.Lerp(a, b, DoEase(ease, by));
    }
    
    public static Vector2 DoEase(Ease ease, Vector2 a, Vector2 b, float by) {
        return new Vector2(DoEase(ease, a.X, b.X, by), DoEase(ease, a.Y, b.Y, by));
    }
    
    private static float InQuad(float t) => t * t;
    private static float OutQuad(float t) => 1 - InQuad(1 - t);
    private static float InCubic(float t) => t * t * t;
    private static float OutCubic(float t) => 1 - InQuad(1 - t);
    public static float InQuart(float t) => t * t * t * t;
    public static float OutQuart(float t) => 1 - InQuart(1 - t);
    public static float InQuint(float t) => t * t * t * t * t;
    public static float OutQuint(float t) => 1 - InQuint(1 - t);
    public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
    public static float OutExpo(float t) => 1 - InExpo(1 - t);
    public static float InCirc(float t) => 1 - (float)Math.Sqrt(1 - t * t);
    public static float OutCirc(float t) => (float)Math.Sqrt(1 - (t - 1) * (t - 1));
    public static float InElastic(float t) => (float)Math.Sin(13 * Math.PI / 2 * t) * (float)Math.Pow(2, 10 * (t - 1));
    public static float OutElastic(float t) => 1 - InElastic(1 - t);
    public static float InBack(float t) => t * t * t - t * (float)Math.Sin(t * Math.PI);
    public static float OutBack(float t) => 1 - InBack(1 - t);
    public static float InBounce(float t) => 1 - OutBounce(1 - t);
    public static float OutBounce(float t) {
        float div = 2.75f;
        float mult = 7.5625f;

        if (t < 1 / div) {
            return mult * t * t;
        }

        if (t < 2 / div) {
            t -= 1.5f / div;
            return mult * t * t + 0.75f;
        }

        if (t < 2.5 / div) {
            t -= 2.25f / div;
            return mult * t * t + 0.9375f;
        }

        t -= 2.625f / div;
        return mult * t * t + 0.984375f;
    }
    public static float InFlash(float t) => t < 0.5 ? 0 : 1;
    public static float OutFlash(float t) => t < 0.5 ? 1 : 0;
    public static float InOutFlash(float t) => t < 0.5 ? 0 : 1;
}

public enum Ease {
    Linear,
    
    InSine,
    OutSine,
    InOutSine,
    
    InQuad,
    OutQuad,
    InOutQuad,
    
    InCubic,
    OutCubic,
    InOutCubic,
    
    InQuart,
    OutQuart,
    InOutQuart,
    
    InQuint,
    OutQuint,
    InOutQuint,
    
    InExpo,
    OutExpo,
    InOutExpo,
    
    InCirc,
    OutCirc,
    InOutCirc,
    
    InElastic,
    OutElastic,
    InOutElastic,
    
    InBack,
    OutBack,
    InOutBack,
    
    InBounce,
    OutBounce,
    InOutBounce,
    
    Flash,
    InFlash,
    OutFlash,
    InOutFlash,
}
