using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;

namespace Adofai.Engine.Actions; 

public class MoveCamera : Action {
    private Player p;
    
    private float duration;
    private string relativeTo;
    private Ease ease;

    private Vector2? offset;
    private float? rotation;
    private float? zoom;
    
    private Vector2 originalOffset;
    private float originalRotation;
    private float originalZoom;
    
    private float elapsed;
    
    public MoveCamera(Ease ease, float duration, Vector2? offset, string relativeTo, float? rotation, float? zoom) {
        this.ease = ease;
        this.duration = duration;
        this.offset = offset;
        this.relativeTo = relativeTo;
        this.rotation = rotation;
        this.zoom = zoom;
    }

    public override void OnLoad(AdofaiFile l) {
        duration /= l.Bps;
    }

    public override void OnLand(Player p, AdofaiFile l) {
        this.p = p;
        
        // Tile: Make Camera relative to where the tile is currently, dont change when the tile moves
        // Player: Make Camera follow last tile, change when the tile changes
        // Last Position: Make Camera follow 2 tiles beforehand, change when the tile changes
        // Global: Not relative to anything

        if (relativeTo != null) {
            switch (relativeTo) {
                case "Player":
                    p.CameraFollowType = FollowType.Position;
                    break;
                
                case "LastPosition":
                    p.CameraFollowType = FollowType.LastPosition;
                    break;
                
                case "Tile":
                case "Global":
                    p.CameraFollowType = FollowType.None;
                    p.CameraTarget = Vector2.Zero;
                    break;
            }   
        }

        if (offset.HasValue) {
            offset *= AdofaiFile.staticSpacing;
            originalOffset = p.CameraOffset * AdofaiFile.staticSpacing;
            MainGame.UpdateEvent += OffsetUpdate;
        }
        if (rotation.HasValue) {
            originalRotation = Camera.RotationDegrees;
            MainGame.UpdateEvent += RotationUpdate;
        }
        if (zoom.HasValue) {
            zoom = AdofaiFile.ConvertFromAdofaiZoom(zoom.Value);
            originalZoom = Camera.Zoom;
            MainGame.UpdateEvent += ZoomUpdate;
        }

        MainGame.UpdateEvent += Update;
    }

    private void Update() {
        elapsed += GTime.Delta;

        if (elapsed > duration) {
            MainGame.UpdateEvent -= Update;

            if (offset.HasValue) {
                p.CameraOffset = offset.Value;
                MainGame.UpdateEvent -= OffsetUpdate;
            }
            if (rotation.HasValue) {
                Camera.RotationDegrees = rotation.Value;
                MainGame.UpdateEvent -= RotationUpdate;
            }
            if (zoom.HasValue) {
                Camera.Zoom = zoom.Value;
                MainGame.UpdateEvent -= ZoomUpdate;
            }
        }
    }

    // All Possible System.InvalidOperationExceptions are not possible because they wouldn't be subscribed if they were
    private void OffsetUpdate() {
        p.CameraOffset = Easings.DoEase(ease, originalOffset, offset.Value, elapsed / duration);
    }
    
    private void RotationUpdate() {
        Camera.RotationDegrees = Easings.DoEase(ease, originalRotation, rotation.Value, elapsed / duration);
    }

    private void ZoomUpdate() {
        Camera.Zoom = Easings.DoEase(ease, originalZoom, zoom.Value, elapsed / duration);
    }
}