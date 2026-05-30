using UnityEngine;

public class EditorMarker : MonoBehaviour
{
	public GameObject posMarkerObject;

	public GameObject dirMarkerObject;

	private void Update()
	{
		if ((bool)posMarkerObject)
		{
			posMarkerObject.transform.position = UpdatePosition();
		}
		if ((bool)dirMarkerObject)
		{
			dirMarkerObject.transform.position = UpdatePosition();
		}
	}

	protected virtual Vector3 UpdatePosition()
	{
		return Vector3.zero;
	}

	protected virtual Vector3 UpdateDirection()
	{
		return Vector3.zero;
	}
}
