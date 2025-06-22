using BepInEx.Configuration;

namespace SatisfyingOverlay.Models;

public class SettingsModel
{
	public static SettingsModel Instance { get; private set; }

	private SettingsModel(ConfigFile configFile)
	{
		showVideo = configFile.Bind(
			"Main", 
			"Show ADHD overlay", 
			true, 
			"Show videos in overlay in raid. Will applied when restart raid");
		
		positionX = configFile.Bind(
			"UI",
			"Position X",
			0f,
			new ConfigDescription(
				"Move video by X axis",
				new AcceptableValueRange<float>(-1000f, 1000f)
			));
		
		positionY = configFile.Bind(
			"UI",
			"Position Y",
			0f,
			new ConfigDescription(
				"Move video by Y axis",
				new AcceptableValueRange<float>(-1000f, 1000f)
			));
		
		scaleWidth = configFile.Bind(
			"UI", 
			"Scale Width", 
			320f, 
			new ConfigDescription(
				"Scale video by horizonal size",
				new AcceptableValueRange<float>(0f, 2000f)
			));
		
		scaleHeight = configFile.Bind(
			"UI", 
			"Scale Height", 
			180f, 
			new ConfigDescription(
				"Scale video by vertical size",
				new AcceptableValueRange<float>(0f, 2000f)
			));
		
		transparency = configFile.Bind(
			"UI", 
			"Transparency", 
			0.5f, 
			new ConfigDescription(
				"Transparency scale video in raid",
				new AcceptableValueRange<float>(0f, 1f)
			));
	}
	
	public static SettingsModel Create(ConfigFile configFile)
	{
		if (Instance != null)
		{
			return Instance;
		}
		return Instance = new SettingsModel(configFile);
	}

	public ConfigEntry<bool> showVideo;
	public ConfigEntry<bool> updateButton;
	public ConfigEntry<float> positionX;
	public ConfigEntry<float> positionY;
	public ConfigEntry<float> scaleWidth;
	public ConfigEntry<float> scaleHeight;
	public ConfigEntry<float> transparency;
}