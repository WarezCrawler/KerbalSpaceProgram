using UnityEngine;

namespace KerbalEngineer.Settings;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class GeneralSettings : MonoBehaviour
{
	private readonly string fileName = "GeneralSettings.xml";

	public static SettingHandler Handler { get; private set; }

	public static GeneralSettings Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void OnDisable()
	{
		if (Handler != null)
		{
			Handler.Save(fileName);
		}
	}

	private void OnEnable()
	{
		Handler = SettingHandler.Load(fileName);
	}
}
