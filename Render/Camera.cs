using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adofai.Render; 

public static class Camera {
    public static float Zoom { get; set; }
    public static float RotationRadians { get; set; }
    public static float RotationDegrees {
        get => MathHelper.ToDegrees(RotationRadians);
        set => RotationRadians = MathHelper.ToRadians(value);
    }
    
    public static Vector2 Position { get; set; }
    public static Rectangle Bounds { get; private set; }
    public static Rectangle VisibleArea { get; private set; }
    public static Matrix Transform { get; private set; }

    public static void Init(Viewport viewport) {
        Bounds = viewport.Bounds;
        Reset();
    }

    public static void Reset() {
        Zoom = 1f;
        Position = Vector2.Zero;
        RotationRadians = 0;
    }

    private static void UpdateVisibleArea() {
        Matrix inverseViewMatrix = Matrix.Invert(Transform);

        Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        Vector2 tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
        Vector2 bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
        Vector2 br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

        Vector2 min = new Vector2(
            MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
            MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
        Vector2 max = new Vector2(
            MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
            MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
        
        VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
    }

    private static void UpdateMatrix() {
        Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateScale(Zoom) * Matrix.CreateRotationZ(RotationRadians) *
                    Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, Zoom * -0.5f));
        
        UpdateVisibleArea();
    }

    public static void UpdateCamera(Viewport bounds) {
        Bounds = bounds.Bounds;
        UpdateMatrix();
    }
}