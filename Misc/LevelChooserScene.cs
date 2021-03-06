using System;
using System.IO;
using Adofai.Engine;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NativeFileDialogSharp;
using Keyboard = Adofai.Render.Keyboard;

namespace Adofai.Misc; 

public class LevelChooserScene : IScene {
    string levelName;
    
    public void LoadScene() {
        MainGame.UpdateEvent += Update;
        MainGame.DrawEvent += Draw;
        MainGame.BackgroundColor = new Color(0.2f, 0.2f, 0.2f);

        if (Program.FilePath == null) {
            levelName = "None";
        }
        else {
            levelName = Path.GetFileNameWithoutExtension(Program.FilePath);
        }
    }

    private void Update() {
        if (Keyboard.IsKeyPressed(Keys.A)) { // Auto
            Program.Auto = !Program.Auto;
        }

        if (Keyboard.IsKeyPressed(Keys.L)) { // Level
            DialogResult result = Dialog.FileOpen();
                
            if (!result.IsOk) return;
            
            Program.FilePath = result.Path;
            levelName = Path.GetFileNameWithoutExtension(result.Path);
        }
        
        if (Keyboard.IsKeyPressed(Keys.OemOpenBrackets)) { // Offset Decrease
            Program.OffsetOption -= 0.01f;
        }
        
        if (Keyboard.IsKeyPressed(Keys.OemCloseBrackets)) { // Offset Increase
            Program.OffsetOption += 0.01f;
        }

        if (Keyboard.IsKeyPressed(Keys.Space) && Program.FilePath != null) {
            Logger.Info("Loading file: " + Program.FilePath);
            SceneLoader.LoadScene(new Level());
        }
    }

    private void Draw() {
        ARender.DrawString($"Level Select", Align.Centre, new Point(0,-512), 1);
        
        ARender.DrawString($"Auto (A): {Program.Auto}", Align.Centre, new Point(0,-120), 2);
        ARender.DrawString($"Level (L): {levelName}", Align.Centre, new Point(0, 0), 2);
        ARender.DrawString($"Offset ([+]): {Math.Round(Program.OffsetOption * 1000)}ms", Align.Centre, new Point(0, 120), 2);
        ARender.DrawString($"Usually half of the adofai offset", Align.Centre, new Point(0, 240), 6);
        
        ARender.DrawString("Press Space to Play",   Align.Centre, new Point(0, 512-90), 3,
            color: Program.FilePath != null ? Color.White : Color.Gray);
    }
}