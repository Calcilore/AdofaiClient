using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Action = Adofai.Engine.Actions.Action;

namespace Adofai.Engine; 

/// <summary>
/// A Class for managing loading .ADOFAI Files
/// </summary>
public class AdofaiFile {
    public List<Tile> TileData { get; }              // The Tile Data
    public float Bps { get; private set; }           // beats per second
    public int Version { get; private set; }         // Adofai file version number
    public float Offset { get; private set; }        // offset, in seconds

    public string FolderPath { get; private set; }   // The path to the folder containing the .ADOFAI file
    public string FilePath { get; private set; }     // The path to the .ADOFAI file
    public string SongPath { get; private set; }     // The path to the song audio file

    // --------------------
    //   STATIC FUNCTIONS
    // --------------------

    public static float ConvertFromAdofaiZoom(float zoom) {
        return 162f / zoom; // Simplified from 1f / (zoom / 100f) * 1.62f
    }

    // ----------------
    //   FILE LOADING
    // ----------------
    
    // Loading Variables for Events / Actions
    public bool Twirl;
    public Vector2 position;
    public float spacing = 100f;
    public const float staticSpacing = 100f;
    public float opacity = 1f;
    public float rotation = 0f;
    public float scale = 1f;
    public float loadBps;
    
    // Loading Variable for Player
    public Vector2 CameraStartPos { get; private set; }
    public Vector2 CameraTarget { get; private set; }
    public Vector2 CameraOffset { get; private set; }
    public FollowType CameraFollowType { get; private set; }
    public float CameraSpeed { get; private set; }

    public AdofaiFile(string fileName) {
        JsonElement doc = LoadDocument(fileName).RootElement;
        JsonElement settings = doc.GetProperty("settings");

        // Get Settings
        GetSettings(settings);
        
        // Get Angle Data
        float[] angleData = GetPathData(doc);

        // Get Action Data
        List<JsonElement>[] actions = GetActions(doc, angleData.Length);

        // Get Tile Data
        TileData = new List<Tile>();
        GetTileData(angleData, actions);
        
        // Get Camera Data
        GetCameraData(settings);
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

    private void GetSettings(JsonElement settings) {
        Version = settings.GetProperty("version").GetInt32();
        Bps = settings.GetProperty("bpm").GetSingle() * MainGame.BpsC;
        loadBps = Bps;
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

        Logger.Error($"Path Character {path} is unknown!");
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
        float timeAngle = -1f;
        float timeSeconds = -1f / loadBps;
        position = new Vector2(0, 0);

        for (int i = 0; i < angleData.Length; i++) {
            Tile t = new Tile();
            Tile prev = TileData.Count > 0 ? TileData[i-1] : null;
            
            // If tile is endspin
            if (Math.Abs(angleData[i] - 5.55) < 0.01) {
                // Get previous Tile (the midspin)
                Tile prev2 = TileData[i-2];
                
                // Set previous tile to be midspin and current to be endspin
                prev.MidspinType = MidspinType.Midspin;
                t.MidspinType = MidspinType.Endspin;
                
                // make endspin angle opposite of midspin angle
                t.Angle = (prev.Angle + 1) % 2;

                // Timing is the same as the previous tile
                t.TimingAngle = prev.TimingAngle;
                t.TimingSeconds = prev.TimingSeconds;

                // Set time as the same as 2 tiles beforehand
                timeAngle = prev2.TimingAngle;
                timeSeconds = prev2.TimingSeconds;
                preAngle = prev2.Angle;
                
                // Calculate Angle Difference
                float angleDiff = CalculateAngleDiff(prev2.Angle, prev.Angle);
                
                // account for extra loops introduced by midspin
                if (angleDiff > 1) {
                    timeAngle += 2f;
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

                timeAngle += angleDiff;
                t.TimingAngle = timeAngle;
                
                timeSeconds += angleDiff / loadBps;
                t.TimingSeconds = timeSeconds;

                // Position:
                if (i != 0) position += Vector2.UnitX.RotateRadians(angleData[i] * -Util.Pi) * spacing;
                t.Position = position;
                
                // Update previous angle:
                preAngle = angleData[i];
            }

            // will be overridden by below lines next loop, except for the ending tile
            t.opacity = opacity;
            t.AddedRotation = rotation;
            t.Scale = scale;
            
            if (prev != null) {
                prev.opacity = t.opacity;
                prev.AddedRotation = t.AddedRotation;
                prev.Scale = t.Scale;
            }

            TileData.Add(t);
            
            // Add Events:
            foreach (JsonElement json in actions[i]) {
                bool success = Action.jsonToAction(json, this, i, out Action a);
                if (a != null) t.Actions.Add(a);
                else if (!success) 
                    Logger.Warn($"Action {json.GetProperty("eventType").GetString()} failed to load, ignoring");
            }
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

    private void GetCameraData(JsonElement settings) {
        float zoom = settings.GetProperty("zoom").GetSingle();
        Camera.Zoom = ConvertFromAdofaiZoom(zoom);

        Camera.RotationDegrees = settings.GetProperty("rotation").GetSingle();
        
        CameraStartPos = Vector2.Zero;
        CameraTarget = Vector2.Zero;
        CameraSpeed = 1.5f;
        
        CameraOffset = Util.GetVector2FromJson(settings.GetProperty("position")) * staticSpacing;

        switch (settings.GetProperty("relativeTo").GetString()) {
            case "Player":
                CameraFollowType = FollowType.Position;
                break;

            case "Tile":
            case "Global":
                CameraFollowType = FollowType.None;
                CameraTarget = Vector2.Zero;
                break;
        }
    } 
}