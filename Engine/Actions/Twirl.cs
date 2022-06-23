using Microsoft.Xna.Framework.Graphics;
using Texture = Adofai.Render.Texture;

namespace Adofai.Engine.Actions; 

public class Twirl : Action {
    public override void OnLoad(AdofaiFile l) {
        l.Twirl = !l.Twirl;
    }

    public override void OnLand(Player p, AdofaiFile _) {
        p.Twirl = !p.Twirl;
    }

    public override Texture GetIcon() {
        return Texture.Twirl1;
    }
}