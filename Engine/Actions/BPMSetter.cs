using Adofai.Misc;
using Adofai.Render;

namespace Adofai.Engine.Actions; 

public class BPMSetter : Action {
    private float amount;
    
    public BPMSetter(float amount) {
        this.amount = amount;
    }
    
    public override Texture GetIcon() {
        return Texture.Bunny;
    }

    private void DoThing(ref float var) {
        Logger.Debug($"BPM Setter: {var} -> {amount}");
        var = amount;
    }

    public override void OnLand(Player _, AdofaiFile l) {
        DoThing(ref l.Bps);
    }

    public override void OnLoad(AdofaiFile l) {
        DoThing(ref l.loadBps);
    }
}