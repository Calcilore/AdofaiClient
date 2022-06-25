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
        
        if (auto || nextTile.MidspinType == MidspinType.Endspin ? angle > timing : 
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