using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Adofai.Render; 

public static class Assets {
    public static Texture2D[] textures;
    private static ContentManager Content;
    
    private static int i = 0;

    public static Texture2D GetTexture(Texture t) {
        return textures[(int)t - 1];
    }
    
    private static void LoadTexture(string name) {
        textures[i] = Content.Load<Texture2D>(name);
        i++;
    }
    
    public static void Load() {
        Content = MainGame.Game.Content;
        
        textures = new Texture2D[(int)Texture.Count - 1];
        
        LoadTexture("Events/Twirl1");
        LoadTexture("Events/Twirl2");
        LoadTexture("Events/Bunny");
    }
}

public enum Texture {
    None,
    Twirl1,
    Twirl2,
    Bunny,
    Count // Texture count, includes None.
}
