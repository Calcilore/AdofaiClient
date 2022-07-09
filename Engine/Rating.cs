using System;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;

namespace Adofai.Engine;

public enum Rating {
    Perfect,
    EPerfect,
    LPerfect,
    Early,
    Late,
    EarlyMiss,
    LateMiss
}

public class RatingText {
    public Point pos;
    
    private readonly string text;
    private readonly float angle;
    private readonly Color color;
    private readonly float creationTime;

    private static Rating CalculateRatingAngle(float timing) {
        float absTiming = Math.Abs(timing);
        
        if (absTiming < 1f / 6f) { // Perfect
            return Rating.Perfect;
        } 
        if (absTiming < 0.25) { // E/L Perfect
            return timing > 0 ? Rating.LPerfect : Rating.EPerfect;
        } 
        if (absTiming < 1f / 3f) { // Early / Late
            return timing > 0 ? Rating.Late : Rating.Early;
        }
        
        // Miss 
        return timing > 0 ? Rating.LateMiss : Rating.EarlyMiss;
    }
    
    private static Rating CalculateRatingSeconds(float timing, float bps) {
        timing /= bps;
        float absTiming = Math.Abs(timing);

        if (absTiming < 0.03226) { // Perfect
            return Rating.Perfect;
        } 
        if (absTiming < 0.04839) { // E/L Perfect
            return timing > 0 ? Rating.LPerfect : Rating.EPerfect;
        } 
        if (absTiming < 0.06452) { // Early / Late
            return timing > 0 ? Rating.Late : Rating.Early;
        }
        
        
        
        // Miss 
        return timing > 0 ? Rating.LateMiss : Rating.EarlyMiss;
    }
    
    public static Rating CalculateRating(float timing, float bps) {
        Rating a = bps > 310 * MainGame.BpsC ? CalculateRatingSeconds(timing, bps) : CalculateRatingAngle(timing);
        return bps > 310 * MainGame.BpsC ? CalculateRatingSeconds(timing, bps) : CalculateRatingAngle(timing);
    }

    public static Rating GetRatingAndCreateRatingText(float timing, bool createRatingText, float bps, out RatingText ratingText) {
        ratingText = null;
        
        Rating rating = CalculateRating(timing, bps);
        
        float angle = 0f;
        Color color;
        
        switch (rating) {
            case Rating.Perfect: // Perfect
                color = new Color(95, 255, 78);
                break;
            case <= Rating.LPerfect: // E/L Perfect
                color = new Color(252, 255, 77);
                angle = 0.1308995f;
                break;
            case <= Rating.Late: // Early / Late
                color = new Color(255, 111, 77);
                angle = 0.2617995f;
                break;
            default: // Miss
                return rating;
        }

        if (timing > 0) angle *= -1;

        if (createRatingText) 
            ratingText = new RatingText(rating.ToString(), angle, color, new Point(0));
        
        return rating;
    }

    private RatingText(string text, float angle, Color color, Point pos) {
        MainGame.DrawEvent += Draw;
        
        this.text = text;
        this.angle = angle;
        this.color = color;
        this.pos = pos;
        this.creationTime = GTime.Total;
    }

    private void Draw() {
        float opacity = MathHelper.Min(creationTime + 1.3f - GTime.Total, 1f);
        if (opacity < 0) {
            MainGame.DrawEvent -= Draw;
            return;
        }
        
        ARender.DrawString(text, Align.Centre, pos, 6, color * opacity, rotation:angle);
    }
}
