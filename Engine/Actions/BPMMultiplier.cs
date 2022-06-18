using System;
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

    public override void OnLand(Player _, Level l) {
        Console.WriteLine($"Multiplier: {l.bps} * {amount} -> {l.bps * amount}");
        l.bps *= amount;
    }
}