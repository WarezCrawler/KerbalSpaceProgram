using UnityEngine;

namespace ns2;

public class AnchoredDialogHost : MonoBehaviour
{
	public AnchoredDialog host;

	public Callback OnHostLateUpdate;

	private void LateUpdate()
	{
		if (host != null)
		{
			OnHostLateUpdate();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
