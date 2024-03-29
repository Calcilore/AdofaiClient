﻿using System;
using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adofai; 

public class MainGame : Game {
    public const float BpsC = 1f / 60f;
    
    public static MainGame Game;
    public delegate void UpdateEventD();
    public static UpdateEventD StaticUpdateEvent;
    public static UpdateEventD UpdateEvent;
    public static UpdateEventD StaticDrawEvent;
    public static UpdateEventD DrawEvent;
    public static UpdateEventD StaticDrawHudEvent;
    public static UpdateEventD DrawHudEvent;
    
    public static GraphicsDeviceManager Graphics;
    public static SpriteBatch SpriteBatch;

    public static Color BackgroundColor;
    
    private FPSCounter fpsCounter;

    public MainGame() {
        Game = this;
        
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        IsFixedTimeStep = false;
    }

    protected override void Initialize() { 
        fpsCounter = new FPSCounter();
        
        ARender.Init();
        StaticUpdateEvent += Keyboard.Update;
        StaticUpdateEvent += Mouse.Update;
        StaticUpdateEvent += fpsCounter.Update;
        StaticDrawHudEvent += fpsCounter.Draw;

        base.Initialize();
    }

    protected override void LoadContent() {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        for (int i = 0; i < ARender.Fonts.Length; i++) {
            ARender.Fonts[i] = Content.Load<SpriteFont>("Fonts/Font" + i);
        }
        
        SceneLoader.LoadScene(new LoadingScene());
    }

    protected override void Update(GameTime gameTime) {
        GTime.FromGameTime(gameTime);

        StaticUpdateEvent?.Invoke();
        UpdateEvent?.Invoke();

        Camera.UpdateCamera(GraphicsDevice.Viewport);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(BackgroundColor);

        ARender.DrawPre();
        SpriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack, transformMatrix:Camera.Transform);
        DrawEvent?.Invoke();
        StaticDrawEvent?.Invoke();
        SpriteBatch.End();
        
        ARender.DrawPre();
        SpriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack);
        DrawHudEvent?.Invoke();
        StaticDrawHudEvent?.Invoke();
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
