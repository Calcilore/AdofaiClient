using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Adofai.Misc;
using Microsoft.Xna.Framework;
using Action = Adofai.Engine.Actions.Action;

namespace Adofai.Engine; 

/// <summary>
/// A Class for managing loading .ADOFAI Files
/// </summary>
public class AdofaiFile {
    public List<Tile> TileData; // The Tile Data
    public float Bps;           // beats per second
    public int Version;         // Adofai file version number
    public float Offset;        // offset, in seconds

    public string FolderPath;
    public string FilePath;
    public string SongPath;


    // ----------------
    //   FILE LOADING
    // ----------------
    
    // Loading Variables for Events / Actions
    public bool Twirl;
    
    public AdofaiFile(string fileName) {
        JsonElement doc = LoadDocument(fileName).RootElement;

        // Get Settings
        GetSettings(doc);
        
        // Get Angle Data
        float[] angleData = GetPathData(doc);

        // Get Action Data
        List<JsonElement>[] actions = GetActions(doc, angleData.Length);

        // Get Tile Data
        TileData = new List<Tile>();
        GetTileData(angleData, actions);
    }

    // For newer versions of ADOFAI, there is a decorations section, it lacks a comma.
    // In some other files before the decorations section, there is a trailing comma after the actions section.
    private JsonDocument LoadDocument(string path) {
        { // Set some vars
            FileInfo adofaiFile = new FileInfo(path);
            FilePath = path;
            FolderPath = adofaiFile.Directory.FullName;
        }
        
        bool hasErrored = false; // prevent infinite loops by only trying to solve the error once
        string file = File.ReadAllText(path);

        while (true) {
            try {
                // try to read file (will fail on newer versions)
                return JsonDocument.Parse(file, new JsonDocumentOptions() {AllowTrailingCommas = true});
            }
            catch (Exception e) {
                // If it is expected error:
                if (e is not JsonException || !e.Message.StartsWith("'\"' is invalid") || hasErrored) throw;

                hasErrored = true;
                    
                // find line number for error
                int lnIndex = e.Message.IndexOf(':');
                int lnIndexEnd = e.Message.IndexOf(' ', lnIndex + 2);
                int lineNumber = int.Parse(e.Message[(lnIndex + 1)..lnIndexEnd]);

                // add comma to end of error line number, then attempt to read again
                string[] spl = file.Split("\n");
                spl[lineNumber-1] += ",";
                file = string.Join("\n", spl);
            }
        }
    }

    private void GetSettings(JsonElement root) {
        JsonElement settings = root.GetProperty("settings");

        Version = settings.GetProperty("version").GetInt32();
        Bps = settings.GetProperty("bpm").GetSingle() * MainGame.BpsC;
        Offset = settings.GetProperty("offset").GetSingle() / 1000f;
        SongPath = settings.GetProperty("songFilename").GetString();
    }
    
    private float[] GetPathData(JsonElement root) {
        
        
        if (root.TryGetProperty("angleData", out JsonElement element)) {
            // Adofai file values divided by 180, and a 0 added at the start of the array
            float[] angleData = new float[element.GetArrayLength() + 1];
            angleData[0] = 0;

            for (int i = 0; i < element.GetArrayLength(); i++) {
                float f = element[i].GetSingle();
                angleData[i + 1] = f / 180;
            }

            return angleData;
        }

        return GetAngleData(root.GetProperty("pathData"));
    }

    private float[] GetAngleData(JsonElement element) { // for older versions of ADOFAI
        string rawData = element.GetString();

        // Adofai file values divided by 180, and a 0 added at the start of the array
        float[] angleData = new float[rawData.Length + 1];
        angleData[0] = 0;

        for (int i = 0; i < rawData.Length; i++) {
            angleData[i + 1] = GetAngleFromPathChar(rawData[i]);
        }

        return angleData;
    }

    private float GetAngleFromPathChar(char path) {
        switch (path) {
            case 'R': return 0f;
            case 'U': return 0.5f;
            case 'L': return 1f;
            case 'D': return 1.5f;
            case 'E': return 0.25f;
            case 'H': return 0.833333333f;
            case 'M': return 1.833333333f;
            case 'T': return 0.666666667f;
            
            case 'F': return 0f; // Finish TODO: Fix this to be correct
            case '!': return 5.55f; // Midspin (999 / 180)
        }

        return -1;
    }
    
    private List<JsonElement>[] GetActions(JsonElement root, int angleDataLen) {
        List<JsonElement>[] actions = new List<JsonElement>[angleDataLen];

        JsonElement actionsElement = root.GetProperty("actions");
        for (int i = 0; i < actions.Length; i++) {
            actions[i] = new List<JsonElement>();
        }
            
        foreach (JsonElement ele in actionsElement.EnumerateArray()) {
            actions[ele.GetProperty("floor").GetInt32()].Add(ele);
        }

        return actions;
    }
    
    private void GetTileData(float[] angleData, List<JsonElement>[] actions) { 
        // Calculate Timings
        float preAngle = 0f;
        float time = -1f;
        Vector2 position = new Vector2(0, 0);

        for (int i = 0; i < angleData.Length; i++) {
            Tile t = new Tile();
            
            // If tile is endspin
            if (Math.Abs(angleData[i] - 5.55) < 0.01) {
                // Get previous Tile (the midspin)
                Tile prev = TileData[i-1];
                Tile prev2 = TileData[i-2];
                
                // Set previous tile to be midspin and current to be endspin
                prev.MidspinType = MidspinType.Midspin;
                t.MidspinType = MidspinType.Endspin;
                
                // make endspin angle opposite of midspin angle
                t.Angle = (prev.Angle + 1) % 2;

                // Timing is the same as the previous tile
                t.Timing = prev.Timing;

                // Set time as the same as 2 tiles beforehand
                time = prev2.Timing;
                preAngle = prev2.Angle;
                
                // Calculate Angle Difference
                float angleDiff = CalculateAngleDiff(prev2.Angle, prev.Angle);
                
                // account for extra loops introduced by midspin
                if (angleDiff > 1) {
                    time += 2f;
                }
                
                // Set position to the same as 2 tiles beforehand (tile before midspin)
                position = prev2.Position;
                t.Position = position;
            }
            // if not endspin
            else {
                // Angle:
                t.Angle = angleData[i];
                
                // Timing:
                float angleDiff = CalculateAngleDiff(preAngle, angleData[i]);

                time += angleDiff;
                t.Timing = time;

                // Position:
                if (i != 0) position += Vector2.UnitX.RotateRadians(angleData[i] * -Util.Pi) * t.Size;
                t.Position = position;
                
                // Update previous angle:
                preAngle = angleData[i];
            }

            // Add Events:
            foreach (JsonElement json in actions[i]) {
                Action a = Action.jsonToAction(json, this);
                if (a != null) t.Actions.Add(a);
                else Logger.Warn($"Action {json.GetProperty("eventType").GetString()} failed to load, ignoring");
            }

            TileData.Add(t);
        }
    }
    
    private float CalculateAngleDiff(float preAngle, float angle) {
        float tMul = Twirl ? -1 : 1;
        preAngle *= tMul; angle *= tMul;
        
        // previous angle + 180 degrees - current angle = angle between two points
        float angleDiff = (preAngle + 1 - angle) % 2;
        while (angleDiff <= 0) angleDiff += 2; // Modulo Doesnt Work Fully
        return angleDiff;
    }
}