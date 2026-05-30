using TMPro;
using UnityEngine;

namespace Expansions.Missions.Flow;

public class MEFlowUIText : MonoBehaviour
{
	public TMP_Text text;

	private void Awake()
	{
		if (!ExpansionsLoader.IsExpansionInstalled("MakingHistory"))
		{
			Object.Destroy(this);
		}
	}
}
