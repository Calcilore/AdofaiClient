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

    public void LoadScene() {
        Camera.UseTargetPosition = true;

        AdofaiFile adofaiFile = new AdofaiFile(Program.FilePath);

        p = new Player(adofaiFile);
    }
    
    public void UnloadScene() { }
}