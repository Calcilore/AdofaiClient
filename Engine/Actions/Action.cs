using System.Text.Json;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;

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

            case "PositionTrack": {
                Vector2 offset = TryGetVector2(element, "positionOffset", Vector2.Zero);
                a = new PositionTrack(offset.X, offset.Y,
                    TryGetFloat(element, "opacity", 100f),
                    TryGetFloat(element, "rotation", 0f),
                    TryGetFloat(element, "scale", 100f));
                break;
            }

            case "MoveCamera": {
                float duration = element.GetProperty("duration").GetSingle();
                Vector2? offset = TryGetVector2(element, "position", null);
                string relativeTo = TryGetString(element, "relativeTo", null);
                float? rotation = TryGetFloat(element, "rotation", null);
                float? zoom = TryGetFloat(element, "zoom", null);
                Ease ease = Easings.AdofaiEasingToEnum(element.GetProperty("ease").GetString());
                
                a = new MoveCamera(ease, duration, offset, relativeTo, rotation, zoom);
                break;
            }
        }

        a?.OnLoad(l);
        return a;
    }
    
    private static string TryGetString(JsonElement element, string path, string def) {
        return element.TryGetProperty(path, out element) ? element.GetString() : def;
    }
    
    private static float? TryGetFloat(JsonElement element, string path, float? def) {
        return element.TryGetProperty(path, out element) ? element.GetSingle() : def;
    }
    
    private static float TryGetFloat(JsonElement element, string path, float def) {
        return element.TryGetProperty(path, out element) ? element.GetSingle() : def;
    }
    
    private static Vector2? TryGetVector2(JsonElement element, string path, Vector2? def) {
        return element.TryGetProperty(path, out element) ? Util.GetVector2FromJson(element) : def;
    }

    private static Vector2 TryGetVector2(JsonElement element, string path, Vector2 def) {
        return element.TryGetProperty(path, out element) ? Util.GetVector2FromJson(element) : def;
    }
}