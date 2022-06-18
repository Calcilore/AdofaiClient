using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adofai.Render; 

public static class Mouse {
    public static Point Position { get; private set; }
    public static int X { get => Position.X; }
    public static int Y { get => Position.Y; }
    public static bool LeftButton { get; private set; }
    public static bool RightButton { get; private set; }
    public static bool MiddleButton { get; private set; }
    public static bool XButton1 { get; private set; }
    public static bool XButton2 { get; private set; }
    public static int ScrollPosition { get; private set; }
    public static int ScrollFrame { get; private set; }
        
    public static int LastX { get; private set; }
    public static int LastY { get; private set; }
    public static bool LastLeftButton { get; private set; }
    public static bool LastRightButton { get; private set; }
    public static bool LastMiddleButton { get; private set; }
    public static bool LastXButton1 { get; private set; }
    public static bool LastXButton2 { get; private set; }
    public static int LastScrollPosition { get; private set; }
    public static int LastScrollFrame { get; private set; }

    public static bool LeftButtonPressed { get; private set; }
    public static bool RightButtonPressed { get; private set; }
    public static bool MiddleButtonPressed { get; private set; }
    public static bool XButton1Pressed { get; private set; }
    public static bool XButton2Pressed { get; private set; }

    public static void Update() {
        LastX = X; LastY = Y;
        LastLeftButton = LeftButton;
        LastMiddleButton = MiddleButton;
        LastRightButton = RightButton;
        LastXButton1 = XButton1;
        LastXButton2 = XButton2;
        LastScrollPosition = ScrollPosition;
        LastScrollFrame = ScrollFrame;

        MouseState ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
            
        // funny window things
        //Point pos = ((ms.Position - Game1.RenderRectangle.Location).ToVector2() / Game1.Scaling).ToPoint();
        //pos += RRender.CameraPos.ToPoint();
        Position = ms.Position;

        LeftButton   = ms.LeftButton   == ButtonState.Pressed;
        RightButton  = ms.LeftButton   == ButtonState.Pressed; 
        MiddleButton = ms.MiddleButton == ButtonState.Pressed;
        XButton1     = ms.XButton1     == ButtonState.Pressed;
        XButton2     = ms.XButton2     == ButtonState.Pressed;
            
        ScrollPosition = Microsoft.Xna.Framework.Input.Mouse.GetState().ScrollWheelValue;
        ScrollFrame = ScrollPosition - LastScrollPosition;
            
        LeftButtonPressed = LeftButton && !LastLeftButton;
        RightButtonPressed = RightButton && !LastRightButton;
        MiddleButtonPressed = MiddleButton && !LastMiddleButton;
        XButton1Pressed =  XButton1 && !LastXButton1;
        XButton2Pressed =  XButton2 && !LastXButton2;
    }
}