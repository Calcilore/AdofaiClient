using System;
using Adofai.Audio;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keyboard = Adofai.Render.Keyboard;

namespace Adofai.Misc; 

public static class VolumeManager {
    private static int volume;
    private const float VolumeDispTime = 1.8f;
    private static float lastVolumeTime;

    public static void Init() {
        MainGame.StaticUpdateEvent += Update;
        MainGame.StaticDrawHUDEvent += DrawHud;

        volume = 0;
        AddVolume(80);
        lastVolumeTime = -1000f;
    }
    
    private static void AddVolume(int add) {
        if (Keyboard.IsKeyHeld(Keys.LeftControl)) add /= 5;
        
        volume = Math.Clamp(volume + add, 0, 100);
        lastVolumeTime = GTime.Total + VolumeDispTime;

        AudioManager.SetVolume(volume);
    }
    
    public static void Update() {
        if (Keyboard.IsKeyPressed(Keys.OemMinus)) AddVolume(-10);
        if (Keyboard.IsKeyPressed(Keys.OemPlus)) AddVolume(10);
    }

    public static void DrawHud() {
        if (lastVolumeTime > GTime.Total) {
            ARender.DrawString($"Volume: {volume}", Align.Left, new Point(10), 6);
        }
    }
}