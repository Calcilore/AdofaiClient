using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Action = Adofai.Engine.Actions.Action;

namespace Adofai.Engine; 

public class Level : IScene {
    private Player p;
    public Tile[] Data;
    public float bps;
    
    // Loading Variables for Events / Actions
    public bool Twirl;

    public void LoadScene() {
        Camera.UseTargetPosition = true;
        
        // Open File and deal with missing comma in .adofai files after "actions":
        JsonDocument doc;
        {
            bool hasErrored = false;
            string file = File.ReadAllText("/home/adam/Desktop/test10.adofai");

            while (true) {
                try {
                    doc = JsonDocument.Parse(file);
                    break;
                }
                catch (Exception e) {
                    // If it is expected error:
                    if (e is not JsonException || !e.Message.StartsWith("'\"' is invalid") || hasErrored) throw;

                    hasErrored = true;
                    
                    // find line number for error
                    int lnIndex = e.Message.IndexOf(':');
                    int lnIndexEnd = e.Message.IndexOf(' ', lnIndex + 2);
                    int lineNumber = int.Parse(e.Message[(lnIndex + 1)..lnIndexEnd]);

                    // add comma to end of error line number
                    string[] spl = file.Split("\n");
                    spl[lineNumber-1] += ",";
                    file = string.Join("\n", spl);
                }
            }
        }
        
        // Get Settings
        {
            JsonElement settings = doc.RootElement.GetProperty("settings");
            
            // Get BPM
            bps = settings.GetProperty("bpm").GetSingle() * MainGame.BpsC;
        }

        // Get Angle Data
        // Adofai file values divided by 180, and a 0 added at the start of the array
        JsonElement element = doc.RootElement.GetProperty("angleData");
        float[] angleData = new float[element.GetArrayLength() + 1];
        angleData[0] = 0;

        for (int i = 0; i < element.GetArrayLength(); i++) {
            float f = element[i].GetSingle();
            angleData[i + 1] = f / 180;
        }
        
        // Get Action Data
        List<JsonElement>[] actions = new List<JsonElement>[angleData.Length];

        {
            JsonElement actionsElement = doc.RootElement.GetProperty("actions");
            for (int i = 0; i < actions.Length; i++) {
                actions[i] = new List<JsonElement>();
            }
            
            foreach (JsonElement ele in actionsElement.EnumerateArray()) {
                actions[ele.GetProperty("floor").GetInt32()].Add(ele);
            }
        }

        List<Tile> data = new List<Tile>();

        { // Calculate Timings
            float preAngle = 0f;
            float time = -1f;
            Vector2 position = new Vector2(0, 0);

            for (int i = 0; i < angleData.Length; i++) {
                Tile t = new Tile();
                
                // If tile is endspin
                if (Math.Abs(angleData[i] - 5.55) < 0.01) {
                    // Get previous Tile (the midspin)
                    Tile prev = data[i-1];
                    
                    // Set previous tile to be midspin and current to be endspin
                    prev.MidspinType = MidspinType.Midspin;
                    t.MidspinType = MidspinType.Endspin;
                    
                    // make endspin angle opposite of midspin angle
                    t.Angle = (prev.Angle + 1) % 2;

                    // Timing is the same as the previous tile
                    t.Timing = prev.Timing;
                    
                    // account for extra loops introduced by midspin
                    Console.WriteLine(prev.Angle);
                    if (prev.Angle > 1) {
                        time += 1f;
                    }
                    
                    // Set position to the same as 2 tiles beforehand (tile before midspin)
                    position = data[i-2].Position;
                    t.Position = position;
                }
                // if not endspin
                else {
                    // Angle:
                    t.Angle = angleData[i];
                    
                    // Timing:
                    float tMul = Twirl ? -1 : 1;
                    float angleDiff = DoTheAngleStuff(preAngle * tMul, angleData[i] * tMul);

                    time += angleDiff;
                    t.Timing = time;

                    // Position:
                    if (i != 0) position += Vector2.UnitX.RotateRadians(angleData[i] * -Util.Pi) * t.Size;
                    t.Position = position;
                }
                
                // Add Events:
                foreach (JsonElement json in actions[i]) {
                    Console.WriteLine(json);
                    Action a = Action.jsonToAction(json, this);
                    if (a != null) t.Actions.Add(a);
                    else Console.WriteLine("Error: Action failed to load, ignoring");
                }

                // Update previous angle:
                preAngle = angleData[i];
                data.Add(t);
                
                //Console.WriteLine($"Tile Timing: {t.Timing}, Position: {t.Position}");
            }
        }
        
        Data = data.ToArray();
        
        p = new Player(this);
    }

    private float DoTheAngleStuff(float preAngle, float angle) {
        // previous angle + 180 degrees - current angle = angle between two points
        float angleDiff = (preAngle + 1 - angle) % 2;
        while (angleDiff <= 0) angleDiff += 2; // Modulo Doesnt Work Fully
        return angleDiff;
    }

    public void UnloadScene() { }
}