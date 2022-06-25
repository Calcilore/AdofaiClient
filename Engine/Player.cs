using System;
using System.IO;
using Adofai.Audio;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Action = Adofai.Engine.Actions.Action;
using Keyboard = Adofai.Render.Keyboard;

namespace Adofai.Engine; 

public class Player {
    private static readonly Color[] Colors = new[] { Color.Red, Color.Blue};
    
    private AdofaiFile level;

    private float angle;
    private int tile;
    private bool finished = true;
    private Point pos;
    private bool dead;
    private bool auto;
    
    // Event Variables
    public bool Twirl;
    
    private float lastTime;

    public Player(AdofaiFile level) {
        this.level = level;
        this.auto = Program.Auto;
        
        MainGame.UpdateEvent += Update;
        MainGame.DrawEvent += Draw;

        lastTime = 0;
        
        AudioManager.LoadSong(Path.Join(level.FolderPath, level.SongPath), 60);
        AudioManager.Play();
        AudioManager.SetPause(true);
        AudioManager.Offset = 1f / level.Bps - level.Offset;
        AudioManager.SetVolume(80);
    }

    private void Update() {
        if (Keyboard.IsKeyPressed(Keys.Space)) {
            finished = false;
            AudioManager.SetPause(false);
        }

        Tile curTile = level.TileData[tile];
        angle = (AudioManager.GetFrameTimeOffset() - lastTime) * level.Bps;

        if (finished) return;

        Tile nextTile = level.TileData[tile + 1];
        
        float timing = (nextTile.Timing - curTile.Timing);
        
        if (auto ? angle > timing : 
                   Math.Abs(angle - timing) < 0.5f && Keyboard.PressedKeys.Length > 0) {
            lastTime += timing / level.Bps;
        
            tile++;

            foreach (Action action in nextTile.Actions) {
                action.OnLand(this, level);
            }

            if (nextTile.MidspinType == MidspinType.None) {
                pos = nextTile.Position.ToPoint();
                angle = 0; // remove one frame of wrong angle after moving   
            }
            else if (nextTile.MidspinType == MidspinType.Midspin) angle = 1f;

            if (tile+1 >= level.TileData.Count) {
                finished = true;
            }
        }
        else if (angle - timing > 1f) {
            dead = true;
        }
    }

    private void Draw() {
        for (int i = 0; i < level.TileData.Count; i++) {
            Tile data = level.TileData[i];
            
            //ARender.DrawBlankCentered(new Rectangle(data.Position.ToPoint(), new Point(60)), Color.Black); /* Debug
            Rectangle drawRect = new Rectangle(
                data.Position.ToPoint() - new Vector2(50, 0).Rotate(data.Angle * -180).ToPoint(),
                new Point(140, 50));
            ARender.DrawBlank(drawRect, data.MidspinType == MidspinType.Endspin ? Color.Aqua : new Color(30, 30, 30), 
                rotation:data.Angle*-180, origin:new Vector2(.5f, .5f)); 
            ARender.DrawBlank(new Rectangle(drawRect.Location.Add(0), new Point(120, 30)),
                rotation:data.Angle*-180, origin:new Vector2(.5f, .5f), color:new Color(50, 50, 50));
            /**/

            if (i != 0 && level.TileData[i-1].Actions.Count > 0) {
                Texture icon = level.TileData[i-1].Actions[0].GetIcon();
                if (icon != Texture.None) {
                    ARender.Draw(icon, new Rectangle(level.TileData[i-1].Position.ToPoint().Sub(20), new Point(60)));
                }
            }

            //if (i == tile) Console.WriteLine(data.Angle * 180);

            //ARender.DrawString(data.Timing.ToString(), Align.Left, data.Position.ToPoint(), 6, depth:.8f);
        }

        // make midspins not flash colors for one frame
        int tCol = tile + (level.TileData[tile].MidspinType == MidspinType.Midspin ? 1 : 0);

        ARender.DrawBlankCentered(new Rectangle(pos, new Point(40)), Colors[tCol % 2]);
        if (!dead)
            ARender.DrawBlankCentered(
                new Rectangle(
                    (Vector2.UnitX * -100).Rotate((angle * (Twirl ? -1 : 1) - level.TileData[tile].Angle) * 180).ToPoint() + pos, 
                    new Point(40)), 
                Colors[(tCol + 1) % 2], rotation:(angle * (Twirl ? -1 : 1) - level.TileData[tile].Angle) * 180
            );
        Camera.TargetPosition = pos.ToVector2();
    }
}