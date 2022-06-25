using Adofai.Misc;
using Adofai.Render;

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