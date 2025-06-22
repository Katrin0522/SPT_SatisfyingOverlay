using BepInEx;
using BepInEx.Logging;
using SatisfyingOverlay.Core;
using SatisfyingOverlay.Models;

namespace SatisfyingOverlay
{
    [BepInPlugin("katrin0522.github.SatisfyingOverlay", "Kat.SatisfyingOverlay", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.KmyTarkovApi", "1.4.0")]
    public class SatisfyingOverlayPlugin : BaseUnityPlugin
    {
        private SettingsModel settings;
        private ADHDManager manager;
        
        public static ManualLogSource LogSource;
        
        private void Awake()
        {
            settings = SettingsModel.Create(Config);
            manager = ADHDManager.Create(Logger);
            
            settings.positionX.SettingChanged += (_, __) =>
            {
                manager.UpdatePosition(settings.positionX.Value, settings.positionY.Value);
            };
            
            settings.positionY.SettingChanged += (_, __) =>
            {
                manager.UpdatePosition(settings.positionX.Value, settings.positionY.Value);
            };
            
            settings.scaleWidth.SettingChanged += (_, __) =>
            {
                manager.UpdateScale(settings.scaleWidth.Value, settings.scaleHeight.Value);
            };
            
            settings.scaleHeight.SettingChanged += (_, __) =>
            {
                manager.UpdateScale(settings.scaleWidth.Value, settings.scaleHeight.Value);
            };
            
            settings.transparency.SettingChanged += (_, __) =>
            {
                manager.UpdateTransparency(settings.transparency.Value);
            };
            
            LogSource = Logger;
            LogSource.LogInfo("SatisfyingOverlay successful loaded!");
        }
    }
}
