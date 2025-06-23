using System;
using BepInEx.Configuration;

namespace SatisfyingOverlay.Models
{
    public class VideoConfigSlot
    {
        public string NameVideoSlot;
        public ConfigEntry<bool> Enabled;
        public ConfigEntry<VideoPreset> Preset;
        public ConfigEntry<string> FileName;
        public ConfigEntry<float> PositionX;
        public ConfigEntry<float> PositionY;
        public ConfigEntry<float> Width;
        public ConfigEntry<float> Height;
        public ConfigEntry<float> Transparency;
    }
    
    [Serializable]
    public enum VideoPreset
    {
        SoapCutting,
        RugWashing,
        KineticSand,
        SlimePoke,
        Custom
    }
}

