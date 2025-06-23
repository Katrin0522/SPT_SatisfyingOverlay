using System;
using System.Collections.Generic;
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
				"Show ADHD overlay", 
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
					Preset = configFile.Bind(
                        section,
                        "Preset",
                        VideoPreset.SoapCutting,
                        new ConfigDescription(
	                        "Preset video to show", 
	                        null, 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 6
	                        })),
					FileName = configFile.Bind(
                        section,
                        "FileName",
                        "soap.mp4",
                        new ConfigDescription(
							"Video file for slot {i}", 
							null, 
							new ConfigurationManagerAttributes
							{
								Order = 5
							})),
					PositionX = configFile.Bind(
                        section,
                        "PositionX",
                        0f,
                        new ConfigDescription("X Position", new AcceptableValueRange<float>(-2000f, 2000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 4
	                        })),
					PositionY = configFile.Bind(
                        section,
                        "PositionY",
                        0f,
                        new ConfigDescription("Y Position", new AcceptableValueRange<float>(-2000f, 2000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 3
	                        })),
					Width = configFile.Bind(
                        section,
                        "Width",
                        320f,
                        new ConfigDescription("Scale Width", new AcceptableValueRange<float>(0f, 3000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 2
	                        })),
					Height = configFile.Bind(
                        section,
                        "Height",
                        180f,
                        new ConfigDescription("Scale Height", new AcceptableValueRange<float>(0f, 3000f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 1
	                        })),
					Transparency = configFile.Bind(
                        section,
                        "Transparency",
                        0.8f,
                        new ConfigDescription("Transparency", new AcceptableValueRange<float>(0f, 1f), 
	                        new ConfigurationManagerAttributes
	                        {
		                        Order = 0
	                        }))
				};
				Slots.Add(slot);
			}
		}
		
		public static string GetFileName(VideoPreset preset)
		{
			return preset switch
			{
				VideoPreset.SoapCutting => "soap.mp4",
				VideoPreset.RugWashing => "rug.mp4",
				VideoPreset.KineticSand => "sand.mp4",
				VideoPreset.SlimePoke => "slime.mp4",
				VideoPreset.Custom => "custom.mp4",
				_ => ""
			};
		}
	}
}