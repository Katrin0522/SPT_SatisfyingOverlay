using BepInEx.Configuration;

namespace SatisfyingOverlay.Models;

public class SettingsModel
{
	public static SettingsModel Instance { get; private set; }

	private SettingsModel(ConfigFile configFile)
	{
		triggerButton = configFile.Bind("Main", "Play ADHD Video", false, "Нажми true → плеер появится");
		positionX = configFile.Bind(
			"UI",
			"Position X",
			0f,
			new ConfigDescription(
				"Позиция по X",
				new AcceptableValueRange<float>(-1000f, 1000f)
			));
		positionY = configFile.Bind(
			"UI",
			"Position Y",
			0f,
			new ConfigDescription(
				"Позиция по Y",
				new AcceptableValueRange<float>(-1000f, 1000f)
			));
		scaleWidth = configFile.Bind(
			"UI", 
			"Scale Width", 
			320f, 
			new ConfigDescription(
				"Scale Width",
				new AcceptableValueRange<float>(0f, 2000f)
			));
		scaleHeight = configFile.Bind(
			"UI", 
			"Scale Height", 
			180f, 
			new ConfigDescription(
				"Scale Height",
				new AcceptableValueRange<float>(0f, 2000f)
			));
		// updateButton = configFile.Bind("UI", "Setup Position", false, "Click for update");
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00004E35 File Offset: 0x00003035
	public static SettingsModel Create(ConfigFile configFile)
	{
		if (Instance != null)
		{
			return Instance;
		}
		return Instance = new SettingsModel(configFile);
	}

	public ConfigEntry<bool> triggerButton;
	public ConfigEntry<bool> updateButton;
	public ConfigEntry<float> positionX;
	public ConfigEntry<float> positionY;
	public ConfigEntry<float> scaleWidth;
	public ConfigEntry<float> scaleHeight;
}