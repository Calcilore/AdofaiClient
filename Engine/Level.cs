using System;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;

namespace Adofai.Engine; 

public class Level : IScene {
    private Player p;
    private AdofaiFile level;

    private string chars = "";
    private float lastTime;

    public void LoadScene() {
        MainGame.DrawEvent += Draw;
        MainGame.UpdateEvent += Update;

        level = new AdofaiFile(Program.FilePath);

        p = new Player(level);

        lastTime = GTime.Total;
    }

    private void Update() {
        if (Keyboard.PressedKeys.Length > 0) {
            if (GTime.Total - lastTime > 0.3f) {
                chars = Keyboard.PressedKeys[0].ToString();
            }
            else {
                chars += Keyboard.PressedKeys[0].ToString();
            }

            if (chars == "EASINGS") {
                SceneLoader.LoadScene(new EasingTest());
            }

            lastTime = GTime.Total;
        }
    }

    private void Draw() {
        for (int i = 0; i < level.TileData.Count; i++) {
            Tile data = level.TileData[i];
            Tile nextData = i+1 < level.TileData.Count ? level.TileData[i + 1] : data;
            
            // Outline
            Rectangle drawRect = new Rectangle(data.Position.ToPoint(), new Vector2(75, 50).Mul(data.Scale).ToPoint());
            Color color = new Color(0.2f, 0.2f, 0.2f) * data.opacity;
            
            ARender.DrawBlankCentered(drawRect, color, rotation:data.Angle * -180 - data.AddedRotation, 
                origin: new Vector2(0.25f,0));
            
            ARender.DrawBlankCentered(drawRect, color, rotation:nextData.Angle * -180 - data.AddedRotation, 
                origin: new Vector2(-0.25f,0));
            
            // Inner Portion
            drawRect = new Rectangle(data.Position.ToPoint(), new Vector2(70, 40).Mul(data.Scale).ToPoint());
            color = new Color(0.25f, 0.25f, 0.25f) * data.opacity;
            
            ARender.DrawBlankCentered(drawRect, color, rotation:data.Angle * -180 - data.AddedRotation, 
                origin: new Vector2(0.25f,0));
            
            ARender.DrawBlankCentered(drawRect, color, rotation:nextData.Angle * -180 - data.AddedRotation, 
                origin: new Vector2(-0.25f,0));

            if (i != 0 && level.TileData[i-1].Actions.Count > 0) {
                Texture icon = level.TileData[i-1].Actions[0].GetIcon();
                if (icon != Texture.None) {
                    ARender.Draw(icon, new Rectangle(level.TileData[i-1].Position.ToPoint().Sub(20), new Point(60)));
                }
            }

            //ARender.DrawString(data.Timing.ToString(), Align.Left, data.Position.ToPoint(), 6, depth:.8f);
        }
    }
}