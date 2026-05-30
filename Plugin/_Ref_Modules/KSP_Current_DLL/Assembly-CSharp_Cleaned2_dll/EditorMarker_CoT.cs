using UnityEngine;

public class EditorMarker_CoT : EditorMarker
{
	private static Ray CoT;

	private static float t;

	private static CenterOfThrustQuery tQry = new CenterOfThrustQuery();

	public static Vector3 Pos => CoT.origin;

	public static Vector3 Dir => CoT.direction;

	private void Update()
	{
		if (!(EditorLogic.fetch == null))
		{
			CoT = FindCoT();
			if ((bool)posMarkerObject)
			{
				posMarkerObject.transform.position = CoT.origin;
			}
			if ((bool)dirMarkerObject && CoT.direction != Vector3.zero)
			{
				dirMarkerObject.transform.forward = CoT.direction;
			}
		}
	}

	public static Ray FindCoT()
	{
		t = 0f;
		Vector3 origin = Vector3.zero;
		Vector3 direction = Vector3.zero;
		recurseParts(EditorLogic.RootPart, ref origin, ref direction, ref t);
		if ((bool)EditorLogic.SelectedPart && !EditorLogic.fetch.ship.Contains(EditorLogic.SelectedPart) && (bool)EditorLogic.SelectedPart.potentialParent)
		{
			recurseParts(EditorLogic.SelectedPart, ref origin, ref direction, ref t);
			for (int i = 0; i < EditorLogic.SelectedPart.symmetryCounterparts.Count; i++)
			{
				recurseParts(EditorLogic.SelectedPart.symmetryCounterparts[i], ref origin, ref direction, ref t);
			}
		}
		if (t != 0f)
		{
			float num = 1f / t;
			origin *= num;
			direction *= num;
			return new Ray(origin, direction);
		}
		return new Ray(Vector3.zero, Vector3.zero);
	}

	private static void recurseParts(Part part, ref Vector3 origin, ref Vector3 direction, ref float t)
	{
		int count = part.Modules.Count;
		while (count-- > 0)
		{
			if (part.Modules[count] is IThrustProvider thrustProvider)
			{
				tQry.Reset();
				thrustProvider.OnCenterOfThrustQuery(tQry);
				origin += tQry.pos * tQry.thrust;
				direction += tQry.dir * tQry.thrust;
				t += tQry.thrust;
			}
		}
		for (int i = 0; i < part.children.Count; i++)
		{
			recurseParts(part.children[i], ref origin, ref direction, ref t);
		}
	}
}
