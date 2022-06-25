using Adofai.Audio;
using Adofai.Engine;
using Adofai.Misc;
using Microsoft.Xna.Framework;

namespace Adofai.Render; 

public class LoadingScene : IScene {
    public void LoadScene() {
        MainGame.UpdateEvent += Update;
        MainGame.DrawEvent += Draw;
    }

    private void Update() {
        AudioManager.Init();
        Assets.Load();

        SceneLoader.LoadScene(new Level());
    }

    private void Draw() {
        ARender.DrawString("Loading...", Align.Centre, Point.Zero, 1);
    }
}