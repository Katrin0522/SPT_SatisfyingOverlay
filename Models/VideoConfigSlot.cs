using BepInEx.Configuration;

namespace SatisfyingOverlay.Models
{
    public class VideoConfigSlot
    {
        public string NameVideoSlot;
        public ConfigEntry<bool> Enabled;
        public ConfigEntry<string> FileName;
        public ConfigEntry<float> PositionX;
        public ConfigEntry<float> PositionY;
        public ConfigEntry<float> Width;
        public ConfigEntry<float> Height;
        public ConfigEntry<float> Transparency;
        public ConfigEntry<bool> AudioEnable;
    }
}

