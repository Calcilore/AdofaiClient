using System;
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
        MainGame.DrawHUDEvent = null;
        Camera.Reset();
        
        // Load new scene
        currentScene = scene;
        currentScene.LoadScene();
    }
}

public interface IScene {
    public void LoadScene();
    public void UnloadScene();
}
