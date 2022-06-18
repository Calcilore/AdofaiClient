using Microsoft.Xna.Framework;

namespace Adofai; 

public static class GTime {
    public static float Delta;
    public static float Total;

    public static void FromGameTime(GameTime g) {
        Delta = (float)g.ElapsedGameTime.TotalSeconds;
        Total = (float)g.TotalGameTime.TotalSeconds;
    }
}