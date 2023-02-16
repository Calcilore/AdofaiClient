using System;
using Adofai.Audio;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keyboard = Adofai.Render.Keyboard;

namespace Adofai.Misc; 

public static class VolumeManager {
    public static int Volume { get; private set; }
    private static float songVolume;
    private static float soundVolume;
    
    private const float VolumeDispTime = 1.8f;
    private static float lastVolumeTime;

    public static void Init() {
        MainGame.StaticUpdateEvent += Update;
        MainGame.StaticDrawHudEvent += DrawHud;

        Volume = 0;
        songVolume = 0.6f;
        soundVolume = 1;
        AddVolume(80);
        lastVolumeTime = -1000f;
    }

    public static float GetFloatVolume() {
        return Volume / 100f;
    }

    public static float GetSoundVolume() {
        return GetFloatVolume() * soundVolume;
    }
    
    public static int GetSongVolume() {
        return (int)(Volume * songVolume);
    }
    
    private static void AddVolume(int add) {
        if (Keyboard.IsKeyHeld(Keys.LeftControl)) add /= 5;

        if (Keyboard.IsKeyHeld(Keys.LeftShift))
            songVolume = Math.Clamp(songVolume + (add / 100f), 0, 1);
        else if (Keyboard.IsKeyHeld(Keys.RightShift))
            soundVolume = Math.Clamp(soundVolume + (add / 100f), 0, 1);
        else
            Volume = Math.Clamp(Volume + add, 0, 100);
        
        lastVolumeTime = GTime.Total + VolumeDispTime;

        AudioManager.SetVolume(GetSongVolume());
    }
    
    public static void Update() {
        if (Keyboard.IsKeyPressed(Keys.OemMinus)) AddVolume(-10);
        if (Keyboard.IsKeyPressed(Keys.OemPlus)) AddVolume(10);
    }

    public static void DrawHud() {
        if (lastVolumeTime > GTime.Total) {
            ARender.DrawString($"Volume: {Volume}\n", Align.Left, new Point(10), 5);
            ARender.DrawString($"Sounds: {Math.Round(soundVolume * 100)}\n" +
                               $"Song: {Math.Round(songVolume * 100)}\n", Align.Left, new Point(10,40), 6);
        }
    }
}