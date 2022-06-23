using System.Text.Json;
using Adofai.Render;

namespace Adofai.Engine.Actions; 

public class Action {
    public virtual void OnLand(Player p, AdofaiFile l) {}
    public virtual void OnLoad(AdofaiFile l) {}
    
    public virtual Texture GetIcon() { return Texture.None; }
    

    public static Action jsonToAction(JsonElement element, AdofaiFile l) {
        Action a = null;
        
        switch (element.GetProperty("eventType").GetString()) {
            case "Twirl": {
                a = new Twirl();
                break;
            }

            case "SetSpeed": {
                if (element.GetProperty("speedType").GetString() == "Bpm")
                    a = new BPMSetter(element.GetProperty("beatsPerMinute").GetSingle() * MainGame.BpsC);
                else
                    a = new BPMMultiplier(element.GetProperty("bpmMultiplier").GetSingle());
                break;
            }
        }

        a?.OnLoad(l);
        return a;
    }
}