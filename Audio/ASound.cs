using Adofai.Misc;
using Adofai.Render;
using Microsoft.Xna.Framework.Audio;

namespace Adofai.Audio; 

public static class ASound {
    public static void Play(SoundEffect sound, float pitch = 0f, float pan = 0f) {
        sound.Play(VolumeManager.GetSoundVolume(), pitch, pan);
    }
    
    public static void Play(Sound sound, float pitch = 0, float pan = 0f) {
        Play(Assets.GetSound(sound), pitch, pan);
    }
}