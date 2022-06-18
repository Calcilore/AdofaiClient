using System;
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

    public override void OnLand(Player _, Level l) {
        Console.WriteLine($"{l.bps} -> {amount}");
        l.bps = amount;
    }
}