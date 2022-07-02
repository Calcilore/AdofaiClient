using Adofai.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adofai.Render; 

public static class ARender {
    public const float DefaultDrawDepth = 0.5f;
    public const float DepthAddAmount = 0.0000001f;
    
    public static Texture2D Blank;
    private static float depthAdd = 0f;

    public static SpriteFont[] Fonts = new SpriteFont[7];

    public static void Init() {
        Camera.Init(MainGame.Graphics.GraphicsDevice.Viewport);
        
        Blank = new Texture2D(MainGame.Graphics.GraphicsDevice, 1, 1);
        Blank.SetData(new Color[] {Color.White});
    }

    public static void DrawPre() {
        depthAdd = 0f;
    }
    
    // -------------------
    //    DRAW ADVANCED
    // -------------------
    
    public static void Draw(Texture2D texture, Rectangle pos, Rectangle source, Color? color = null, float rotation = 0f,
        Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        
        MainGame.SpriteBatch.Draw(
            texture, 
            pos, 
            source, 
            color.GetValueOrDefault(Color.White), 
            MathHelper.ToRadians(rotation), 
            origin.GetValueOrDefault(Vector2.Zero), 
            spriteEffects, 
            MathHelper.Min(depth + depthAdd, 1f) 
            // Min is because if you draw too many items, depthAdd will make the depth more than 1, making the object not draw
        );
        
        depthAdd += DepthAddAmount;
    }
    

    // -----------------
    //    DRAW NORMAL
    // -----------------
    
    public static void Draw(Texture2D texture, Rectangle pos, Color? color = null, float rotation = 0f,
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        Draw(texture, pos, 
            new Rectangle(new Point(0), pos.Size), color, rotation, origin, spriteEffects, depth);
    }

    public static void Draw(Texture texture, Rectangle pos, Color? color = null, float rotation = 0f,
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        
        Draw(Assets.GetTexture(texture), pos, color, rotation, origin, spriteEffects, depth);
    }
    
    public static void DrawCentered(Texture2D texture, Rectangle pos, Color? color = null, float rotation = 0f, 
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        Draw(texture, pos, color, rotation, 
            origin.GetValueOrDefault(Vector2.Zero) + new Vector2(.5f, .5f), 
            spriteEffects, depth
        );
    }

    public static void DrawCentered(Texture texture, Rectangle pos, Color? color = null, float rotation = 0f,
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        Draw(Assets.GetTexture(texture), pos, color, rotation, origin, spriteEffects, depth);
    }
    
    // ----------------
    //    DRAW BLANK
    // ----------------
    
    public static void DrawBlank(Rectangle pos, Color? color = null, float rotation = 0f, 
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        Draw(Blank, pos, new Rectangle(0,0,1,1), color, rotation, origin, spriteEffects, depth);
    }

    public static void DrawBlankCentered(Rectangle pos, Color? color = null, float rotation = 0f, 
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {
        DrawBlank(pos, color, rotation, 
            origin.GetValueOrDefault(Vector2.Zero) + new Vector2(.5f, .5f), 
            spriteEffects, depth
        );
    }
    
    // -----------------
    //    DRAW STRING
    // -----------------
    
    public static Vector2 MeasureString(string text, int scale) {
        return Fonts[scale].MeasureString(text);
    }
    
    public static void DrawString(string text, Align justify, Point pos, int scale, Color? color = null, float rotation = 0f, 
    Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = DefaultDrawDepth) {

        // Align.Left subtracts 0, Align.Centre subtracts size/2, Align.Right subtracts size
        Vector2 textSize = MeasureString(text, scale);
        pos.X -= (int)(textSize.X * ((int)justify / 2f));
        
        MainGame.SpriteBatch.DrawString(
            Fonts[scale], 
            text,
            pos.ToVector2(), 
            color.GetValueOrDefault(Color.White), 
            rotation, 
            origin.GetValueOrDefault(Vector2.Zero), 
            Vector2.One,
            spriteEffects, 
            MathHelper.Min(depth + depthAdd, 1f) 
            // Min is because if you draw too many items, depthAdd will make the depth more than 1, making the object not draw
        );

        depthAdd += DepthAddAmount;
    }
}

public enum Align {
    Left = 0,
    Centre = 1,
    Right = 2
}
