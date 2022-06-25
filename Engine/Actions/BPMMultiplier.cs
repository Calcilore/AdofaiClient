using System;
using Adofai.Misc;
using Adofai.Render;

namespace Adofai.Engine.Actions; 

public class BPMMultiplier : Action {
    private float amount;
    
    public BPMMultiplier(float amount) {
        this.amount = amount;
    }
    
    public override Texture GetIcon() {
        return Texture.Bunny;
    }

    public override void OnLand(Player _, AdofaiFile l) {
        Logger.Debug($"BPM Multiplier: {l.Bps} * {amount} -> {l.Bps * amount}");
        l.Bps *= amount;
    }
}