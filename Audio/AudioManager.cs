using System;
using Adofai.Misc;
using LibVLCSharp.Shared;
using LogLevel = Adofai.Misc.LogLevel;

namespace Adofai.Audio; 

public static class AudioManager {
    private static LibVLC _libVLC;

    public static Media Media;
    public static MediaPlayer MediaPlayer;

    public static float bpm;
    public static float bps;

    public static float Offset        = 0f;
    public static float FrameTime     = 0f; // progress in seconds, updates every frame
    public static float LastFrameTime = 0f; // frameTime but last frame
    public static float MusicTime     = 0f; // progress in seconds, updates about every half second, is tied to the song so will always be accurate
    public static float LastMusicTime = 0f; // musicTime but last frame
        
    public static void Init() {
        Logger.Info("Initialising Audio Manager");
        Core.Initialize();
        _libVLC = new LibVLC(false);
        _libVLC.Log += HandleLog;
        MediaPlayer = new MediaPlayer(_libVLC);
        MainGame.StaticUpdateEvent += Update;
    }

    public static void Update() {
        LastMusicTime = MusicTime;
        LastFrameTime = FrameTime;

        FrameTime += GTime.Delta * MediaPlayer.Rate;
        MusicTime = MediaPlayer.Time / 1000f;

        // if music time changes, then set frametime to musictime (sync to song)
        if (Math.Abs(MusicTime - LastMusicTime) > 0.05) {
            FrameTime = MusicTime;
        }
            
        // If it is paused
        if (!MediaPlayer.IsPlaying) {
            FrameTime -= GTime.Delta * MediaPlayer.Rate;
        }
    }
    
    public static float GetFrameTimeOffset() {
        return FrameTime + Offset;
    }
    
    public static float GetMusicTimeOffset() {
        return MusicTime + Offset;
    }

    public static float GetBeatTime() {
        return FrameTime * bps;
    }

    public static float GetSpeed() {
        return MediaPlayer.Rate;
    }
        
    public static int GetVolume() {
        return MediaPlayer.Volume;
    }
        
    public static void SetVolume(int volume) {
        MediaPlayer.Volume = volume;
    }

    public static bool IsPlaying() {
        return MediaPlayer.IsPlaying;
    }

    public static void Play() {
        MediaPlayer.Play();
    }
        
    public static void Stop() {
        MediaPlayer.Stop();
    }

    public static void Seek(float time) {
        MediaPlayer.Time = (long) (time * 1000);
    }

    public static void SetPause(bool pause) {
        MediaPlayer.SetPause(pause);
    }

    public static void LoadSong(string song, float bpm, float speed = 1f) {
        //Logger.Info("Loading Song: " + song);
            
        AudioManager.bpm = bpm;
        bps = bpm * MainGame.BpsC;
            
        Media = new Media(_libVLC, song);
        MediaPlayer.Media = Media;

        MediaPlayer.SetRate(speed);
    }
    
    private static void HandleLog(object sender, LogEventArgs args) {
        LogLevel level = args.Level switch {
            LibVLCSharp.Shared.LogLevel.Debug => LogLevel.Debug,
            LibVLCSharp.Shared.LogLevel.Notice => LogLevel.Info,
            LibVLCSharp.Shared.LogLevel.Warning => LogLevel.Warn,
            LibVLCSharp.Shared.LogLevel.Error => LogLevel.Error,
            _ => throw new ArgumentOutOfRangeException()
        };
            
        if (level == LogLevel.Debug) return;
        
        Logger.Log($"VLC: {args.Module ?? "none"}: {args.Message}", level);
    }
}
