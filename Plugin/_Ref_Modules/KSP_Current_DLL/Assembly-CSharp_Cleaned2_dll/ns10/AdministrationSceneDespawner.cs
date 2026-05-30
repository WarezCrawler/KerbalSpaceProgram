using UnityEngine;
using UnityEngine.UI;

namespace ns10;

public class AdministrationSceneDespawner : MonoBehaviour
{
	public Button exitButton;

	private void Start()
	{
		exitButton.onClick.AddListener(BtnExit);
	}

	private void BtnExit()
	{
		GameEvents.onGUIAdministrationFacilityDespawn.Fire();
	}
}
