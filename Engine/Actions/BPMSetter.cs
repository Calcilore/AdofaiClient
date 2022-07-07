using Adofai.Misc;
using Adofai.Render;

namespace Adofai.Engine.Actions; 

public class BPMSetter : Action {
    private float amount;
    private bool isSetter; // Otherwise it's a multiplier
    private Texture icon;
    
    public BPMSetter(float amount, bool isSetter) {
        this.amount = amount;
        this.isSetter = isSetter;
    }
    
    public override Texture GetIcon() {
        return icon;
    }

    private float DoThing(float var) {
        if (isSetter) {
            Logger.Debug($"BPM Setter: {var} -> {amount}");
            return amount;
        }

        Logger.Debug($"BPM Multiplier: {var} * {amount} -> {var * amount}");
        return var * amount;
    }

    public override void OnLand(Player _, AdofaiFile l) {
        l.Bps = DoThing(l.Bps);
    }

    public override void OnLoad(AdofaiFile l, int index) {
        float newBps = DoThing(l.loadBps);
        
        // Get if snail:
        icon = newBps < l.loadBps ? Texture.Snail : Texture.Bunny;

        l.loadBps = newBps;
    }
}