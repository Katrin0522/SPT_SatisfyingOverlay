using BepInEx;
using BepInEx.Logging;
using SatisfyingOverlay.Core;
using SatisfyingOverlay.Models;

namespace SatisfyingOverlay
{
    // first string below is your plugin's GUID, it MUST be unique to any other mod. Read more about it in BepInEx docs. Be sure to update it if you copy this project.
    [BepInPlugin("katrin0522.github.SatisfyingOverlay", "Kat.SatisfyingOverlay", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.KmyTarkovApi", "1.4.0")]
    public class SatisfyingOverlayPlugin : BaseUnityPlugin
    {
        private SettingsModel settings;
        private ADHDManager manager;
        
        public static ManualLogSource LogSource;

        // BaseUnityPlugin inherits MonoBehaviour, so you can use base unity functions like Awake() and Update()
        private void Awake()
        {
            settings = SettingsModel.Create(Config);
            manager = ADHDManager.Create(Logger);
            
            settings.triggerButton.SettingChanged += (_, __) =>
            {
                if (settings.triggerButton.Value)
                {
                    manager.PlayADHDVideo(settings.positionX.Value, settings.positionY.Value);
                    settings.triggerButton.Value = false;
                }
            };
            
            // updateButton.SettingChanged += (_, __) =>
            // {
            //     if (updateButton.Value)
            //     {
            //         manager.UpdatePosition(positionX.Value, positionY.Value);
            //         manager.UpdateScale(scaleWidth.Value, scaleHeight.Value);
            //         updateButton.Value = false;
            //     }
            // };
            
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
            
            LogSource = Logger;
            LogSource.LogInfo("plugin VideoADHD loaded!");
            
            // new SimplePatch().Enable();
            // new SimplePatchCheck().Enable();
        }
    }
}
