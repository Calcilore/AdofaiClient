using Microsoft.Xna.Framework;

namespace Adofai.Engine.Actions; 

public class PositionTrack : Action {
    private Vector2 offset;
    private float opacity;
    private float rotation;
    private float scale;

    public PositionTrack(float x, float y, float opacity, float rotation, float scale) {
        offset = new Vector2(x, -y);
        this.opacity = opacity / 100f;
        this.rotation = rotation;
        this.scale = scale / 100f;
    }
    
    public override void OnLoad(AdofaiFile l, int _) {
        l.position += offset * l.spacing;
        l.TileData[^1].Position += offset * l.spacing;
        l.opacity = opacity;
        l.rotation = rotation;
        l.scale = scale;
    }
}