using System.Collections.Generic;
using Adofai.Engine.Actions;
using Microsoft.Xna.Framework;

namespace Adofai.Engine; 

public class Tile {
    public float Angle;
    public float AddedRotation;
    public float Timing;
    public Vector2 Position;
    public float opacity;
    public float Scale;
    public MidspinType MidspinType = MidspinType.None;
    public List<Action> Actions = new List<Action>();
}

public enum MidspinType {
    None,
    Midspin,
    Endspin // endspin is the tile that is after the midspin (or the tile that the player lands on after hitting a midspin)
}
