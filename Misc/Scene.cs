using Adofai.Render;
using Microsoft.Xna.Framework;

namespace Adofai.Misc;

public static class SceneLoader {
    public static IScene currentScene;

    public static void LoadScene(IScene scene) {
        // Unload scene and remove subscribers from events (leaving UpdateEvent)
        currentScene?.UnloadScene();
        MainGame.DrawEvent = null;
        MainGame.UpdateEvent = null;
        MainGame.DrawHudEvent = null;

        // Load new scene
        currentScene = scene;
        
        // Finish loading scene next frame to make sure that things running this frame wont interfere with the new scene
        MainGame.UpdateEvent += AddSubscribers;
    }
    
    private static void AddSubscribers() {
        Camera.Reset();
        MainGame.BackgroundColor = Color.CornflowerBlue;
        currentScene.LoadScene();
        MainGame.UpdateEvent -= AddSubscribers;
    }
}

public interface IScene {
    public void LoadScene() { }
    public void UnloadScene() { }
}
