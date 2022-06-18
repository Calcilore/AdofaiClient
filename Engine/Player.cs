using System;
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
    
    private Level level;

    private float angle;
    private int tile;
    private bool finished = true;
    private Point pos;
    
    // Event Variables
    public bool Twirl;

    // Temporary variables for movement
    private float lastTime;
    private int fuck = -1;
    
    public Player(Level level) {
        this.level = level;
        MainGame.UpdateEvent += Update;
        MainGame.DrawEvent += Draw;

        lastTime = 0;
        
        AudioManager.LoadSong("/home/adam/Desktop/crucify.ogg", 60);
        AudioManager.Play();
        AudioManager.SetPause(true);
        AudioManager.Offset = 2f / level.bps;
        AudioManager.SetVolume(80);
    }

    private void Update() {
        fuck--;
        if (Keyboard.IsKeyPressed(Keys.Space)) {
            finished = false;
            AudioManager.SetPause(false);
        }
        
        Console.WriteLine(AudioManager.GetFrameTimeOffset());
        
        Tile curTile = level.Data[tile];
        angle = (AudioManager.GetFrameTimeOffset() - lastTime) * level.bps;

        if (finished) return;

        Tile nextTile = level.Data[tile + 1];
        
        if (angle > (nextTile.Timing - curTile.Timing)) {
            fuck = 5;
            
            lastTime = AudioManager.GetFrameTimeOffset();
            
            tile++;

            foreach (Action action in nextTile.Actions) {
                action.OnLand(this, level);
            }

            if (nextTile.MidspinType == MidspinType.None) {
                pos = nextTile.Position.ToPoint();
                angle = 0; // remove one frame of wrong angle after moving   
            }
            else if (nextTile.MidspinType == MidspinType.Midspin) angle = 1f;

            if (tile+1 >= level.Data.Length) {
                finished = true;
            }
        }
    }

    private void Draw() {
        for (int i = 0; i < level.Data.Length; i++) {
            Tile data = level.Data[i];
            
            ARender.DrawBlankCentered(new Rectangle(data.Position.ToPoint(), new Point(60)), Color.Black); /* Debug
            ARender.DrawBlank(new Rectangle(
                data.Position.ToPoint() - new Vector2(50,0).Rotate(data.Angle*-180).ToPoint(), 
                new Point(140, 50)), data.MidspinType == MidspinType.Endspin ? Color.Aqua : Color.Black, 
                rotation:data.Angle*-180, origin:new Vector2(.5f, .5f)); /**/

            if (data.Actions.Count > 0) {
                Texture icon = data.Actions[0].GetIcon();
                if (icon != Texture.None) {
                    ARender.Draw(icon, new Rectangle(data.Position.ToPoint().Sub(20), new Point(60)));
                }
            }

            //if (i == tile) Console.WriteLine(data.Angle * 180);

            ARender.DrawString(data.Timing.ToString(), Align.Left, data.Position.ToPoint(), 6, depth:.8f);
        }

        // make midspins not flash colors for one frame
        int tCol = tile + (level.Data[tile].MidspinType == MidspinType.Midspin ? 1 : 0);

        ARender.DrawBlankCentered(new Rectangle(pos, new Point(40)), Colors[tCol % 2]);
        ARender.DrawBlankCentered(
            new Rectangle(
                (Vector2.UnitX * -100).Rotate((angle * (Twirl ? -1 : 1) - level.Data[tile].Angle) * 180).ToPoint() + pos, 
                new Point(40)), 
            Colors[(tCol + 1) % 2], rotation:(angle * (Twirl ? -1 : 1) - level.Data[tile].Angle) * 180
        );
        Camera.TargetPosition = pos.ToVector2();
        
        //if (fuck >= 0) ARender.DrawBlank(new Rectangle((int)(-960 + Camera.Position.X), (int)(-540 + Camera.Position.Y), 1920, 1080), Color.Black, depth:.8f);
    }
}