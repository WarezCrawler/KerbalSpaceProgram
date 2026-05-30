using UnityEngine;

namespace ns2;

public abstract class UICameraBase : MonoBehaviour
{
	[SerializeField]
	protected Camera cam;

	private void Reset()
	{
		cam = GetComponent<Camera>();
	}
}
