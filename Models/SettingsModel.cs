using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using KmyTarkovConfiguration.Attributes;

namespace SatisfyingOverlay.Models
{
	public class SettingsModel
	{
		public List<VideoConfigSlot> Slots = new();
		
		public static SettingsModel Instance { get; private set; }
		
		public ConfigEntry<bool> GlobalEnable;

		private SettingsModel(ConfigFile configFile)
		{
			GlobalEnable = configFile.Bind(
				"Main", 
				"Show overlay", 
				true, 
				"Show videos in overlay in raid. Will applied when restart raid");
			
			InitVideoSlots(configFile);
		}
		
		public static SettingsModel Create(ConfigFile configFile)
		{
			if (Instance != null)
			{
				return Instance;
			}
			return Instance = new SettingsModel(configFile);
		}
		
		private void InitVideoSlots(ConfigFile configFile)
		{
			var videoPath = Path.Combine(BepInEx.Paths.PluginPath, "SatisfyingOverlay", "Videos");
			string[] files = Directory.GetFiles(videoPath, "*.mp4")
				.Select(Path.GetFileName)
				.ToArray();
			
			for (int i = 0; i < 10; i++)
			{
				string section = $"Video {i}";
				var slot = new VideoConfigSlot
				{
					NameVideoSlot = section,
					Enabled = configFile.Bind(
						section, 
						"Enabled", 
						false, 
						new ConfigDescription(
							"Enable this video", 
							null, 
							new ConfigurationManagerAttributes
							{
								Order = 7
							})),
					FileName = configFile.Bind(
                        section,
                        "Video",
                        "soap.mp4",
                        new ConfigDescription(
							"Select video. List update need restart game", 
							new AcceptableValueList<string>(files), 
							new ConfigurationManagerAttributes
							{
								Order = 6
							})),
					PositionX = configFile.Bind(
                        section,
                        "PositionX",
                        0f,
                        new ConfigDescription("X Position", new AcceptableValueRange<float>(-2000f, 2000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 5
	                        })),
					PositionY = configFile.Bind(
                        section,
                        "PositionY",
                        0f,
                        new ConfigDescription("Y Position", new AcceptableValueRange<float>(-2000f, 2000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 4
	                        })),
					Width = configFile.Bind(
                        section,
                        "Width",
                        320f,
                        new ConfigDescription("Scale Width", new AcceptableValueRange<float>(0f, 3000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 3
	                        })),
					Height = configFile.Bind(
                        section,
                        "Height",
                        180f,
                        new ConfigDescription("Scale Height", new AcceptableValueRange<float>(0f, 3000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 2
	                        })),
					Transparency = configFile.Bind(
                        section,
                        "Transparency",
                        0.8f,
                        new ConfigDescription("Transparency", new AcceptableValueRange<float>(0f, 1f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 1
	                        })),
					AudioEnable = configFile.Bind(
						section,
						"AudioEnable",
						false,
						new ConfigDescription("AudioEnable", null, 
							new ConfigurationManagerAttributes
							{
								Order = 0
							}))
				};
				Slots.Add(slot);
			}
		}
	}
}