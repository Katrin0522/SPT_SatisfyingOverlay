using BepInEx;
using BepInEx.Logging;
using SatisfyingOverlay.Core;
using SatisfyingOverlay.Models;

namespace SatisfyingOverlay
{
    [BepInPlugin("katrin0522.github.SatisfyingOverlay", "Kat.SatisfyingOverlay", "1.0.2")]
    [BepInDependency("com.kmyuhkyuk.KmyTarkovApi", "1.4.0")]
    public class SatisfyingOverlayPlugin : BaseUnityPlugin
    {
        private SettingsModel _settings;
        private VideoManager _manager;
        private ManualLogSource _logSource;
        
        private void Awake()
        {
            _settings = SettingsModel.Create(Config);
            _manager = VideoManager.Create(Logger);

            _settings.GlobalEnable.SettingChanged += (_, __) =>
            {
                foreach (var slot in _settings.Slots)
                {
                    _manager.UpdateVideoSlot(slot);
                }
            };
            
            foreach (var slot in _settings.Slots)
            {
                slot.Enabled.SettingChanged += (_, __) =>
                {
                    _manager.UpdateVideoSlot(slot);
                };
                
                slot.FileName.SettingChanged += (_, __) =>
                {
                    _manager.UpdateVideoSlot(slot);
                };
                
                slot.PositionX.SettingChanged += (_, __) =>
                {
                    _manager.UpdatePosition(slot);
                };
                
                slot.PositionY.SettingChanged += (_, __) =>
                {
                    _manager.UpdatePosition(slot);
                };
                
                slot.Width.SettingChanged += (_, __) =>
                {
                    _manager.UpdateScale(slot);
                };
                
                slot.Height.SettingChanged += (_, __) =>
                {
                    _manager.UpdateScale(slot);
                };
                
                slot.Transparency.SettingChanged += (_, __) =>
                {
                    _manager.UpdateTransparency(slot);
                };
                
                slot.AudioEnable.SettingChanged += (_, __) =>
                {
                    _manager.UpdateMute(slot);
                };
            }
            
            _logSource = Logger;
            _logSource.LogInfo("SatisfyingOverlay successful loaded!");
        }
    }
}
