using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Adofai.Render; 

public class Keyboard {
    public static Keys[] HeldKeys = Array.Empty<Keys>();
    public static Keys[] LastHeldKeys = Array.Empty<Keys>();
    public static Keys[] PressedKeys = Array.Empty<Keys>();
    public static KeyboardState ks;

    public static void Update() {
        LastHeldKeys = HeldKeys;
            
        ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        HeldKeys = ks.GetPressedKeys();
            
        List<Keys> pressedKeys = new List<Keys>(HeldKeys);
        List<Keys> LHK = new List<Keys>(LastHeldKeys);
        foreach (Keys t in HeldKeys) {
            for (int j = 0; j < LHK.Count; j++) {
                if (LHK[j] == t) {
                    pressedKeys.Remove(t);
                    LHK.Remove(LHK[j]);
                    break;
                }
            }
        }

        PressedKeys = pressedKeys.ToArray();
    }

    public static bool IsKeyPressed(Keys k) {
        return PressedKeys.Contains(k);
    }
    
    public static bool IsKeyReleased(Keys k) {
        return !HeldKeys.Contains(k) && LastHeldKeys.Contains(k);
    }

    public static bool IsKeyHeld(Keys k) {
        return HeldKeys.Contains(k);
    }
}