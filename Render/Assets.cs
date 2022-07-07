using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Adofai.Render; 

public static class Assets {
    private static Texture2D[] textures;
    private static SoundEffect[] sounds;
    private static ContentManager Content;
    
    private static int i = 0;

    public static Texture2D GetTexture(Texture t) {
        return textures[(int)t - 1];
    }
    
    public static SoundEffect GetSound(Sound s) {
        return sounds[(int)s - 1];
    }
    
    private static void LoadTexture(string name) {
        textures[i] = Content.Load<Texture2D>(name);
        i++;
    }
    
    private static void LoadSound(string name) {
        sounds[i] = Content.Load<SoundEffect>(name);
        i++;
    }
    
    public static void Load() {
        Content = MainGame.Game.Content;
        
        textures = new Texture2D[(int)Texture.Count - 1];
        LoadTexture("Events/Twirl1");
        LoadTexture("Events/Twirl2");
        LoadTexture("Events/Bunny");

        i = 0;
        sounds = new SoundEffect[(int)Sound.Count - 1];
        LoadSound("hitsound");
    }
}

public enum Texture {
    None,
    Twirl1,
    Twirl2,
    Bunny,
    Count // Texture count, includes None.
}

public enum Sound {
    None,
    HitSound,
    Count // Texture count, includes None.
}
