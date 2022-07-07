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
    private Vector2 pos;
    private Vector2 lastPos;
    private bool dead;
    private bool auto;
    
    // Hitsounds
    private int hitsoundTile = 1;

    // Event Variables
    public bool Twirl;
    
    // Camera
    public Vector2 CameraStartPos; // Starting position for the Camera
    public Vector2 CameraTarget;
    public Vector2 CameraOffset;
    public FollowType CameraFollowType;
    public float CameraSpeed; // time (in seconds) for the camera to move to the target
    public float CameraTimer; // time (in seconds) since the camera has started moving to the target

    private float lastTime;

    public Player(AdofaiFile level) {
        this.level = level;
        this.auto = Program.Auto;

        // Camera vars stored in level
        CameraStartPos = level.CameraStartPos;
        CameraTarget = level.CameraTarget;
        CameraOffset = level.CameraOffset;
        CameraFollowType = level.CameraFollowType;
        CameraSpeed = level.CameraSpeed;
        CameraTimer = level.CameraTimer;

        MainGame.UpdateEvent += Update;
        MainGame.DrawEvent += Draw;

        lastTime = 0;
        
        AudioManager.LoadSong(Path.Join(level.FolderPath, level.SongPath), 60);
        AudioManager.Play();
        AudioManager.SetPause(true);
        AudioManager.Offset = 1f / level.Bps - level.Offset;
    }

    private void Update() {
        if (Keyboard.IsKeyPressed(Keys.Space)) {
            finished = false;
            AudioManager.SetPause(false);
        }
        
        // Camera
        if (CameraTimer < CameraSpeed) {
            Camera.Position = Vector2.Lerp(CameraStartPos, CameraTarget + CameraOffset, CameraTimer / CameraSpeed);
            CameraTimer += GTime.Delta;

            if (CameraTimer >= CameraSpeed) {
                CameraStartPos = Camera.Position;
            }
        }

        Tile curTile = level.TileData[tile];
        angle = (AudioManager.GetFrameTimeOffset() - lastTime) * level.Bps;

        if (finished) return;

        // Hitsounds
        if (!dead && AudioManager.GetFrameTimeOffset() > level.TileData[hitsoundTile].TimingSeconds) {
            hitsoundTile++;
            ASound.Play(Sound.HitSound);
        }
        
        // The player logic
        Tile nextTile = level.TileData[tile + 1];
        
        float timing = (nextTile.TimingAngle - curTile.TimingAngle);

        if (auto || nextTile.MidspinType == MidspinType.Endspin ? angle > timing : 
                Math.Abs(angle - timing) < 0.5f && Keyboard.PressedKeys.Length > 0) {
            lastTime += timing / level.Bps;

            tile++;
            
            foreach (Action action in nextTile.Actions) {
                action.OnLand(this, level);
            }

            if (nextTile.MidspinType == MidspinType.None) {
                lastPos = pos;
                pos = nextTile.Position;
                angle = 0; // remove one frame of wrong angle after moving   
            }
            else if (nextTile.MidspinType == MidspinType.Midspin) angle = 1f;
            
            // Camera Follow
            if (CameraFollowType == FollowType.Position) {
                CameraTarget = pos;
                CameraStartPos = Camera.Position;
                CameraTimer = 0;
            }
            else if (CameraFollowType == FollowType.LastPosition) {
                CameraTarget = lastPos;
                CameraStartPos = Camera.Position;
                CameraTimer = 0;
            }

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

        ARender.DrawBlankCentered(new Rectangle(pos.ToPoint(), new Point(40)), Colors[tCol % 2]);
        if (!dead)
            ARender.DrawBlankCentered(new Rectangle(pos.ToPoint(), new Point(40)),
                origin:new Vector2(2.5f,0f), 
                rotation:(angle * (Twirl ? -1 : 1) - level.TileData[tile].Angle) * 180, color:Colors[(tCol + 1) % 2]);
    }
}

public enum FollowType {
    None,
    Position,
    LastPosition
}
