using System;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keyboard = Adofai.Render.Keyboard;

namespace Adofai; 

public class EasingTest : IScene {
    private Ease eee = 0;
    
    public void LoadScene() {
        MainGame.DrawEvent += Draw;
        eee = Ease.Linear;
    }

    private void Draw() {
        if (Keyboard.IsKeyPressed(Keys.L)) {
            eee += 1;
            if (!Enum.IsDefined(eee)) {
                eee = 0;
            }
        }
        
        if (Keyboard.IsKeyPressed(Keys.K)) {
            eee--;
            if (!Enum.IsDefined(eee)) {
                eee = Ease.InOutFlash;
            }
        }
        
        ARender.DrawBlank(new Rectangle(-960, 210-540-10-10, 1920, 10), Color.Black);
        ARender.DrawBlank(new Rectangle(-960, 870-540+10, 1920, 10), Color.Black);
        
        const int width = 1146;
        for (int i = 0; i < width; i++) {
            int easeValue = (int)Easings.DoEase(eee, 870-540, 210-540, i / (float)width);
            ARender.DrawBlankCentered(new Rectangle(i-width/2, easeValue, 10, 10));
        }
        
        ARender.DrawString(eee + "\nPress K & L to switch easings", 
            Align.Left, new Point(-950, -530), 5, Color.Black);
    }
}