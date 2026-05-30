using Expansions;
using TMPro;
using UnityEngine;
using ns9;

namespace ns31;

public class VersionInfo : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI version_text;

	private void Start()
	{
		if (version_text != null)
		{
			version_text.text = Versioning.GetVersionStringFull() + " " + Localizer.CurrentLanguage;
			version_text.text += ExpansionsLoader.GetInstalledExpansionsString();
		}
	}
}
