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
    private bool auto;
    
    // Death
    private bool dead;
    private bool dying;
    private float timingDiff;
    
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
        float actualCamSpeed = CameraSpeed / level.Bps;
        
        if (CameraTimer < actualCamSpeed) {
            Camera.Position = Vector2.Lerp(CameraStartPos, CameraTarget + CameraOffset, CameraTimer / actualCamSpeed);
            CameraTimer += GTime.Delta;

            if (CameraTimer >= actualCamSpeed) {
                CameraStartPos = Camera.Position;
            }
        }

        Tile curTile = level.TileData[tile];
        angle = (AudioManager.GetFrameTimeOffset() - lastTime) * level.Bps;

        if (finished) return;

        // Hitsounds
        if (!dead && level.TileData.Count > hitsoundTile && AudioManager.GetFrameTimeOffset() > level.TileData[hitsoundTile].TimingSeconds) {
            hitsoundTile++;
            ASound.Play(Sound.HitSound);
        }
        
        // The player logic
        Tile nextTile = level.TileData[tile + 1];
        
        float timing = nextTile.TimingAngle - curTile.TimingAngle;
        timingDiff = angle - timing;
        bool pressedAButton = Keyboard.PressedKeys.Length > 0;
        
        // Calculate Rating
        Rating rating = RatingText.GetRatingAndCreateRatingText(timingDiff, pressedAButton, level.Bps, out RatingText rt);
        
        if (auto || nextTile.MidspinType == MidspinType.Endspin ? angle > timing : 
                rating < Rating.EarlyMiss && pressedAButton) {
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

            rt.pos = pos.ToPoint() - new Point(0,60);
        }
        else if (timingDiff > 2f) {
            dead = true;
        }
        else if (rating == Rating.LateMiss) {
            dying = true;
        }
    }

    private void Draw() {
        // make midspins not flash colors for one frame
        int tCol = tile + (level.TileData[tile].MidspinType == MidspinType.Midspin ? 1 : 0);

        if (!dead)
            ARender.DrawBlankCentered(
                new Rectangle(pos.ToPoint(), new Vector2(40).Mul(dying ? (2 - timingDiff/2f)/2 : 1f).ToPoint()),
                origin:new Vector2(2.5f,0f) * (dying ? (2 - timingDiff)/2 : 1f),
                rotation:(angle * (Twirl ? -1 : 1) - level.TileData[tile].Angle) * 180, color:Colors[(tCol + 1) % 2]);
        ARender.DrawBlankCentered(new Rectangle(pos.ToPoint(), new Point(40)), Colors[tCol % 2]);
    }
}

public enum FollowType {
    None,
    Position,
    LastPosition
}
