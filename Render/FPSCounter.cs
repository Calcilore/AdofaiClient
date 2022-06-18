using System;
using Microsoft.Xna.Framework;

namespace Adofai.Render; 

public class FPSCounter {
    private const float MsgFrequency = .3f;
    
    private int frames;
    private float last;
    private float now;
    private string msg = "FPS: 0";

    public void Update() {
        now = GTime.Total;
        float elapsed = now - last;
        if (elapsed > MsgFrequency) {
            msg = "FPS: " + Math.Round(frames / elapsed);
            frames = 0;
            last = now;
        }
    }

    public void Draw() {
        ARender.DrawString(msg, Align.Right, new Point(1920-10, 10), 6, Color.Black);
        frames++;
    }
}
