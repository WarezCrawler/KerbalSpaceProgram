using UnityEngine;

public class EditorMarker_CoL : EditorMarker
{
	public Vector3 referenceVelocity = Vector3.up;

	public float referencePitch = 1f;

	public float referenceSpeed = 70f;

	public double refAlt;

	public double refStP;

	public double refDens;

	private Ray CoL;

	private double refTemp;

	private static CenterOfLiftQuery lQry = new CenterOfLiftQuery();

	private void Update()
	{
		if (!(EditorLogic.fetch == null))
		{
			referenceVelocity = EditorLogic.VesselRotation * (Quaternion.AngleAxis(referencePitch, EditorLogic.RootPart.transform.right) * Vector3.up);
			referenceVelocity *= referenceSpeed;
			refAlt = 100.0;
			refStP = FlightGlobals.getStaticPressure(refAlt, Planetarium.fetch.Home);
			refTemp = FlightGlobals.getExternalTemperature(refAlt, Planetarium.fetch.Home);
			refDens = FlightGlobals.getAtmDensity(refStP, refTemp, Planetarium.fetch.Home);
			CoL = FindCoL(referenceVelocity, refAlt, refStP, refDens);
			if ((bool)posMarkerObject)
			{
				posMarkerObject.transform.position = CoL.origin;
			}
			if (CoL.direction == Vector3.zero && dirMarkerObject.activeInHierarchy)
			{
				dirMarkerObject.SetActive(value: false);
			}
			if (CoL.direction != Vector3.zero && !dirMarkerObject.activeInHierarchy)
			{
				dirMarkerObject.SetActive(value: true);
			}
			if ((bool)dirMarkerObject && CoL.direction != Vector3.zero)
			{
				dirMarkerObject.transform.forward = CoL.direction;
			}
		}
	}

	public static Ray FindCoL(Vector3 refVel, double refAlt, double refStp, double refDens)
	{
		Vector3 CoL = Vector3.zero;
		Vector3 DoL = Vector3.zero;
		float t = 0f;
		recurseParts(EditorLogic.RootPart, refVel, ref CoL, ref DoL, ref t, refAlt, refStp, refDens);
		if ((bool)EditorLogic.SelectedPart && !EditorLogic.fetch.ship.Contains(EditorLogic.SelectedPart) && (bool)EditorLogic.SelectedPart.potentialParent)
		{
			recurseParts(EditorLogic.SelectedPart, refVel, ref CoL, ref DoL, ref t, refAlt, refStp, refDens);
			for (int i = 0; i < EditorLogic.SelectedPart.symmetryCounterparts.Count; i++)
			{
				recurseParts(EditorLogic.SelectedPart.symmetryCounterparts[i], refVel, ref CoL, ref DoL, ref t, refAlt, refStp, refDens);
			}
		}
		if (t != 0f)
		{
			float num = 1f / t;
			CoL *= num;
			DoL *= num;
			return new Ray(CoL, DoL);
		}
		return new Ray(Vector3.zero, Vector3.zero);
	}

	private static void recurseParts(Part part, Vector3 refVel, ref Vector3 CoL, ref Vector3 DoL, ref float t, double refAlt, double refStp, double refDens)
	{
		int count = part.Modules.Count;
		while (count-- > 0)
		{
			if (part.Modules[count] is ILiftProvider liftProvider)
			{
				lQry.Reset();
				lQry.refVector = refVel;
				lQry.refAltitude = refAlt;
				lQry.refStaticPressure = refStp;
				lQry.refAirDensity = refDens;
				liftProvider.OnCenterOfLiftQuery(lQry);
				CoL += lQry.pos * lQry.lift;
				DoL += lQry.dir * lQry.lift;
				t += lQry.lift;
			}
		}
		for (int i = 0; i < part.children.Count; i++)
		{
			recurseParts(part.children[i], refVel, ref CoL, ref DoL, ref t, refAlt, refStp, refDens);
		}
	}
}
